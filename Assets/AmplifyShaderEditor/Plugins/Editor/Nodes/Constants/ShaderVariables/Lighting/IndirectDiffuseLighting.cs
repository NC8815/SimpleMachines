// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Indirect Diffuse Light", "Light", "Indirect Lighting", NodeAvailabilityFlags = (int)NodeAvailability.CustomLighting )]
	public sealed class IndirectDiffuseLighting : ParentNode
	{
		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.Tangent;

		private int m_cachedIntensityId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputPort( WirePortDataType.FLOAT3, "RGB" );
			m_autoWrapProperties = true;
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 0";
			m_previewShaderGUID = "b45d57fa606c1ea438fe9a2c08426bc7";
			m_drawPreviewAsSphere = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;

			if( m_cachedIntensityId == -1 )
				m_cachedIntensityId = Shader.PropertyToID( "_Intensity" );

			PreviewMaterial.SetFloat( m_cachedIntensityId, RenderSettings.ambientIntensity );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			// This needs to be rechecked
			//if( m_inputPorts[ 0 ].IsConnected )
			dataCollector.DirtyNormal = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_normalSpace = (ViewSpace)EditorGUILayoutEnumPopup( "Normal Space", m_normalSpace );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePort();
			}
		}

		private void UpdatePort()
		{
			if( m_normalSpace == ViewSpace.World )
				m_inputPorts[ 0 ].ChangeProperties( "World Normal", m_inputPorts[ 0 ].DataType, false );
			else
				m_inputPorts[ 0 ].ChangeProperties( "Normal", m_inputPorts[ 0 ].DataType, false );

			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
				m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
				return "float3(0,0,0)";

			string normal = string.Empty;
			if( m_inputPorts[ 0 ].IsConnected )
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				dataCollector.ForceNormal = true;

				normal = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar );
				if( m_normalSpace == ViewSpace.Tangent )
					normal = "WorldNormalVector( " + Constants.InputVarStr + " , " + normal + " )";
			}
			else
			{
				if( dataCollector.IsFragmentCategory )
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
					if( dataCollector.DirtyNormal )
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
						dataCollector.ForceNormal = true;
					}
				}

				normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
			}


			if( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectDiffuse" + OutputId, "ShadeSH9( float4( " + normal + ", 1 ) )" );
			}
			else
			{
				dataCollector.AddLocalVariable( UniqueId, "UnityGI gi" + OutputId + " = gi;" );
				dataCollector.AddLocalVariable( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT3, "diffNorm"+ OutputId, normal );
				dataCollector.AddLocalVariable( UniqueId, "gi" + OutputId + " = UnityGI_Base( data, 1, diffNorm" + OutputId + " );" );
				dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "indirectDiffuse" + OutputId, "gi" + OutputId + ".indirect.diffuse + diffNorm"+ OutputId+" * 0.0001" );
			}

			return "indirectDiffuse" + OutputId;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 13002 )
				m_normalSpace = (ViewSpace)Enum.Parse( typeof( ViewSpace ), GetCurrentParam( ref nodeParams ) );

			UpdatePort();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalSpace );
		}
	}
}
