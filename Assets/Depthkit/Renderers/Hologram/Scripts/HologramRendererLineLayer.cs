﻿/************************************************************************************

DepthKit Unity SDK License v1
Copyright 2016-2018 Simile Inc. All Rights reserved.  

Licensed under the Simile Inc. Software Development Kit License Agreement (the "License"); 
you may not use this SDK except in compliance with the License, 
which is provided at the time of installation or download, 
or which otherwise accompanies this software in either electronic or hard copy form.  

You may obtain a copy of the License at http://www.depthkit.tv/license-agreement-v1

Unless required by applicable law or agreed to in writing, 
the SDK distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 

************************************************************************************/

using UnityEngine;
using System.Collections;

namespace DepthKit
{
    /// <summary>
    /// HologramRendererLineLayer adds a layer of line based effect to the model
    /// <summary>
    [System.Serializable]
    public class HologramRendererLineLayer : HologramLayer
    {
        public enum LineOrientation
        {
            Vertical,
            Horizontal
        };
        /// <summary>
        /// Orientation switches between horizontal and vertical lines.
        /// <summary>
        [SerializeField]
        private LineOrientation _orientation = LineOrientation.Horizontal;
        public LineOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                _meshDirty = true;
                _orientation = value;
            }
        }

        /// <summary>
        /// Self occlusion property so we do not see occluded lines
        /// <summary>
        public bool _selfOcclusion = false;

        /// <summary>
        /// Line Count controls the number of lines in this layer
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0, 255)]
        public float _lineCount = 10;
        private float _prevLineCount = 10;

        /// <summary>
        /// Line Density controls how many individual line elements make up each line.
        /// Higher numbers means smoother lines but more expensive rendering
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(1, 255)]
        public float _lineDensity = 50;
        private float _prevLineDensity = 50;

        /// <summary>
        /// Opacity if the overall fade of the layer
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(0.0f, 1.0f)]
        public float _opacity = 1.0f;

        /// <summary>
        /// Width thickness of the lines on screen
        /// floating point type to expose to Unity timeline.
        /// <summary>
        [Range(1.0f, 100.0f)]
        public float _width = 1.0f;

        /// <summary>
        /// Line Sprite is a texture that is spread across the line
        /// <summary>
        public Texture2D _lineSprite;

        /// <summary>
        /// Submits the mesh for rendering
        /// <summary>
        public override void Draw(Matrix4x4 transform, Material material)
        {
            if (_mesh == null ||
                _meshDirty ||
                _prevLineCount != _lineCount ||
                _prevLineDensity != _lineDensity)
            {
                BuildMesh();
                _meshDirty = false;
                _prevLineCount = _lineCount;
                _prevLineDensity = _lineDensity;
            }

            if (isActiveAndEnabled && _opacity > 0.0f)
            {
                // push all the items into material property block
                // we use this technique in order to the share the material between all the different layers
                if (_materialBlock == null)
                {
                    _materialBlock = new MaterialPropertyBlock();
                }
                else
                {
                    _materialBlock.Clear();
                }

                if (_selfOcclusion)
                {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                }
                else
                {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                }

                _materialBlock.SetFloat("_MeshDensity", (int)_lineDensity);

                if (_lineSprite != null)
                {
                    _materialBlock.SetTexture("_Sprite", _lineSprite);
                    _materialBlock.SetFloat("_UseSprite", 1.0f);
                }
                else
                {
                    _materialBlock.SetFloat("_UseSprite", 0.0f);
                }
                _materialBlock.SetFloat("_Width", _width/1000.0f);
                _materialBlock.SetFloat("_Opacity", _opacity);

                Graphics.DrawMesh(_mesh, transform, material, 0, null, 0, _materialBlock);
            }
        }

        /// <summary>
        /// Rebuilds the mesh with the current settings
        /// <summary>
        protected void BuildMesh()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
            }
            else
            {
                _mesh.Clear();
            }

            ///////// Build Lines /////////

            int lineDensity = (int)_lineDensity;
            int lineCount = (int)_lineCount;
            int numVerts = lineDensity * lineCount;
            int numSegments = (lineDensity - 1);

            Vector3[] verts = new Vector3[numVerts];
            Vector2[] texcoords = new Vector2[numVerts];
            int[] indices = new int[numVerts];

            int curIndex = 0;

            // swap texture step size based on density
            Vector2 textureStep = new Vector2(1.0f / (float)numSegments, 1.0f / _lineCount);

            for (int line = 0; line < lineCount; line++)
            {
                for (int step = 0; step < lineDensity; step++)
                {
                    Vector2 segment = new Vector2((float)step * textureStep.x, (float)line * textureStep.y);
                    indices[curIndex] = curIndex;
                    verts[curIndex].x = _orientation == LineOrientation.Horizontal ? segment.y : segment.x;
                    verts[curIndex].y = _orientation == LineOrientation.Horizontal ? segment.x : segment.y;
                    verts[curIndex].z = 0;

                    // when we are on the edge of our line lattice, we default uv.x to 1
                    // in the shader we interpret this 1 as "cull segment" as it is the connecting
                    // verts between two lines
                    texcoords[curIndex].x = (step == 0 || step == numSegments) ? 1.0f : 0.0f;

                    curIndex++;
                }
            }

            _mesh.vertices = verts;
            _mesh.uv = texcoords;
            _mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
            _mesh.bounds = _meshBounds;
        }

        /// <summary>
        /// Gets the default shader name for this layer
        /// <summary>
        protected override string DefaultShaderString()
        {
            return "DepthKit/HologramLines";
        }
    }
}