Shader "Show OutSide 2"
{
    SubShader 
	{
        Pass
		{
            Material 
			{
                Diffuse (1,1,1,1)
            }

            Lighting On
            Cull Back
			ZWrite Off
        }
    }
}