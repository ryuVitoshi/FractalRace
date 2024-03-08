// Sphere
// s: radius
float sdSphere(float3 p, float s)
{
	return length(p) - s;
}

// (Infinite) Plane
// n.xyz: normal of the plane (normalized).
// n.w: offset
float sdPlane(float3 p, float4 n)
{
	// n must be normalized
	return dot(p, n.xyz) + n.w;
}

// Box
// b: size of box in x/y/z
float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
}

// Rounded Box
float sdRoundBox( in float3 p, in float3 b, in float r )
{
	float3 q = abs(p) - b;
	return min(max(q.x,max(q.y,q.z)),0.0) + length(max(q,0.0)) - r;
}

// Menger Sponge
/*vec3 map( in vec3 p )
{
	float d = sdBox(p,vec3(1.0));
	vec3 res = vec3( d, 1.0, 0.0, 0.0 );

	float s = 1.0;
	for( int m=0; m<3; m++ )
	{
		vec3 a = mod( p*s, 2.0 )-1.0;
		s *= 3.0;
		vec3 r = abs(1.0 - 3.0*abs(a));

		float da = max(r.x,r.y);
		float db = max(r.y,r.z);
		float dc = max(r.z,r.x);
		float c = (min(da,min(db,dc))-1.0)/s;

		if( c>d )
		{
			d = c;
			res = vec3( d, 0.2*da*db*dc, (1.0+float(m))/4.0, 0.0 );
		}
	}

	return res;
}*/

//MandelBulb
float sdMandelBuld(float3 p)
{
	float power = 3;
	float3 z = p;
	float dr = 1;
	float r;

	for (int i = 0; i < 15; i++)
	{
		r = length(z);
		if (r > 2)
		{
			break;
		}

		float theta = acos(z.z / r) * power;
		float phi = atan2(z.y, z.x) * power;
		float zr = pow(r, power);
		dr = pow(r, power - 1) * power * dr + 1;

		z = zr * float3( sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta) );
		z += p;
	}
	return 0.5 * log(r) * r / dr;
}
// BOOLEAN OPERATORS //

// Union
float4 opU(float4 d1, float4 d2)
{
	return (d1.w < d2.w) ? d1 : d2;
}

// Subtraction
float opS(float d1, float d2)
{
	return max(-d1, d2);
}

// Intersection
float opI(float d1, float d2)
{
	return max(d1, d2);
}

// Mod Position Axis
float pMod1 (inout float p, float size)
{
	float halfsize = size * 0.5;
	float c = floor((p+halfsize)/size);
	p = fmod(p+halfsize,size)-halfsize;
	p = fmod(-p+halfsize,size)-halfsize;
	return c;
}

// SMOOTH BOOLEAN OPERATORS

float4 opUS( float4 d1, float4 d2, float k ) 
{
	float h = clamp( 0.5 + 0.5*(d2.w-d1.w)/k, 0.0, 1.0 );
	float3 color = lerp(d2.rgb, d1.rgb, h);
	float dist = lerp( d2.w, d1.w, h ) - k*h*(1.0-h);
	return float4(color, dist);
}

float opSS( float d1, float d2, float k ) 
{
    float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
    return lerp( d2, -d1, h ) + k*h*(1.0-h); 
}

float opIS( float d1, float d2, float k ) 
{
    float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) + k*h*(1.0-h); 
}