﻿in vec3 aPosition;
in vec2 aPosition2;
in vec2 aTextureCoor;
in vec3 aNormals;
in mat4 InstanceMatrix;
in vec3 Tangent;
in float InstanceSelected;


out float ModelSelected;
out vec3 surfaceNormal;

uniform bool isReflection;
uniform float IsSelected;
//RenderMode
uniform int ShaderType;

//transforms
uniform mat4 ProjectionTransform;
uniform mat4 ViewTransform;
uniform mat4 LocalTransform;

//Environment
uniform vec4 ClipPlanX;
uniform vec4 ClipPlanY;
uniform vec4 ClipPlanZ;
uniform bool EnableClipPlan;



void LoadSurfaceNormal(mat4 modelTransform)
{
	vec3 normalVector=aNormals;
    if(isReflection)
    {
        normalVector = aNormals * vec3(1,-1,1);
    }

    surfaceNormal = (modelTransform *  vec4(normalVector,0)).xyz;	 
}

void CheckClipPlan(vec4 worldPosition)
{
	if(EnableClipPlan)
	   {
			gl_ClipDistance[0] = dot(ClipPlanX,worldPosition);
			gl_ClipDistance[1] = dot(ClipPlanY,worldPosition);
			gl_ClipDistance[2] = dot(ClipPlanZ,worldPosition);
	   }
	   else
	   {
	   //this is required to update openGL there is no clipping here from a previous clipped model
			gl_ClipDistance[0] = 1;
			gl_ClipDistance[1] = 1;
			gl_ClipDistance[2] = 1;
	   }
}

mat4 GetLocalMatrix()
{
	if( determinant(InstanceMatrix)==0)
	{
		return LocalTransform;
	}
	else
	{
		return InstanceMatrix;
	}
}

void SetIsSelected()
{
	if(determinant(InstanceMatrix)==0)
	{
		ModelSelected =  IsSelected;
	}
	else
	{
		ModelSelected =  InstanceSelected;
	}
}

//NormalMap
mat3 GetNormalSpace(vec4 worldPosition, mat4 modelTransform)
{
	 
	mat4 modelViewMatrix = ViewTransform * modelTransform;
	vec3 norm = normalize(surfaceNormal);
	vec3 tang = normalize((modelViewMatrix * vec4(Tangent, 0.0)).xyz);
	vec3 bitAngle = normalize(cross(norm, tang));

	return  mat3(
		tang.x, bitAngle.x, norm.x,
		tang.y, bitAngle.y, norm.y,
		tang.z, bitAngle.z, norm.z
	);	
}