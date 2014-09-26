Shader "Show OutSide"
{
    SubShader 
	{
        Pass
		{
            Material 
			{
                Diffuse (1,0,1,1)
            }
			
            Lighting On
            Cull Back
			ZWrite Off
        }
    }
}