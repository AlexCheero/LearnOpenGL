#version 330 core
struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct DirectionalLight
{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight
{
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    //attenuation:
    float constant;
    float linear;
    float quadratic;
};

struct SpotLight
{
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    //attenuation:
    float constant;
    float linear;
    float quadratic;
};

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

uniform Material material;
uniform SpotLight spotLight;
#define NR_POINT_LIGHTS 4
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform DirectionalLight dirLight;

#define ATTENUATION(light, distance) (1.0 / (light.constant + (light.linear * distance) + (light.quadratic * distance * distance)))

vec3 CalcDiffuse(vec3 norm, vec3 lightDir, vec3 lightDiffuse)
{
    return (max(dot(norm, lightDir), 0.0) * lightDiffuse);
}

vec3 CalcSpecular(vec3 lightSpecular, vec3 viewDir, vec3 lightDir, vec3 norm, vec3 specularColor)
{
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    return spec * lightSpecular * specularColor;
}

vec3 CalcDirLight(DirectionalLight light, vec3 norm, vec3 viewDir, vec3 specularColor)
{
    vec3 lightDir = normalize(-light.direction);
    vec3 diffuse = CalcDiffuse(norm, lightDir, light.diffuse);
    vec3 specular = CalcSpecular(light.specular, viewDir, lightDir, norm, specularColor);
    return light.ambient + diffuse + specular;
}

vec3 CalcPointLight(PointLight light, vec3 norm, vec3 viewDir, vec3 specularColor)
{
    vec3 fragToLight = light.position - FragPos;
    vec3 lightDir = normalize(fragToLight);
    float distance = length(fragToLight);
    float attenuation = ATTENUATION(light, distance);

    vec3 diffuse = CalcDiffuse(norm, lightDir, light.diffuse);
    vec3 specular = CalcSpecular(light.specular, viewDir, lightDir, norm, specularColor);
    return (light.ambient + diffuse + specular) * attenuation;
}

vec3 CalcSpotLight(SpotLight light, vec3 norm, vec3 viewDir, vec3 specularColor)
{
    vec3 fragToLight = light.position - FragPos;
    vec3 lightDir = normalize(fragToLight);
    float distance = length(fragToLight);
    float attenuation = ATTENUATION(light, distance);

    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    vec3 diffuse = CalcDiffuse(norm, lightDir, light.diffuse);
    vec3 specular = CalcSpecular(light.specular, viewDir, lightDir, norm, specularColor);
    return (light.ambient + (diffuse + specular) * intensity) * attenuation;
}

void main()
{
    vec3 norm = normalize(Normal);
    vec3 specularColor = texture(material.specular, TexCoords).rgb;
    vec3 diffuseColor = texture(material.diffuse, TexCoords).rgb;
    vec3 viewDir = normalize(-FragPos);
    
    //vec3 spotLightColor = CalcSpotLight(spotLight, norm, viewDir, specularColor);
    //FragColor = vec4(spotLightColor * diffuseColor, 1.0);

    vec3 result;
    for (int i = 0; i < NR_POINT_LIGHTS; i++)
    {
        result += CalcPointLight(pointLights[i], norm, viewDir, specularColor);
    }
    FragColor = vec4(result * diffuseColor, 1.0);

    //vec3 spotLightColor = CalcDirLight(dirLight, norm, viewDir, specularColor);
    //FragColor = vec4(spotLightColor * diffuseColor, 1.0);
}

/*
void _main()
{
    vec3 diffuseColor = texture(material.diffuse, TexCoords).rgb;
    vec3 ambient = light.ambient * diffuseColor;

    vec3 norm = normalize(Normal);

    //Point light:
    vec3 fragToLight = light.position - FragPos;
    vec3 lightDir = normalize(fragToLight);
    float distance = length(fragToLight);
    float attenuation = 1.0 / (light.constant + (light.linear * distance) + (light.quadratic * distance * distance));
    
    //Directional light:
    //vec3 lightDir = normalize(-light.direction);

    //Spot light:
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);  

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = intensity * diff * light.diffuse * diffuseColor;

    vec3 viewDir = normalize(-FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = intensity * spec * light.specular * texture(material.specular, TexCoords).rgb;

    vec3 result = (ambient + diffuse + specular) * attenuation;
    FragColor = vec4(result, 1.0);
}
*/