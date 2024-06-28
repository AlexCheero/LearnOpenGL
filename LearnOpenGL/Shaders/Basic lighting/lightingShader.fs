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

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

uniform Material material;
uniform PointLight light;
uniform float ambientStrength;
uniform float specularStrength;

void main()
{
    vec3 diffuseColor = texture(material.diffuse, TexCoords).rgb;
    vec3 ambient = ambientStrength * light.ambient * diffuseColor;

    vec3 norm = normalize(Normal);

    //Point light:
    vec3 fragToLight = light.position - FragPos;
    vec3 lightDir = normalize(fragToLight);
    float distance = length(fragToLight);
    float attenuation = 1.0 / (light.constant + (light.linear * distance) + (light.quadratic * distance * distance));
    
    //Directional light:
    //vec3 lightDir = normalize(-light.direction);

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * light.diffuse * diffuseColor;

    vec3 viewDir = normalize(-FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = specularStrength * spec * light.specular * texture(material.specular, TexCoords).rgb;

    vec3 result = (ambient + diffuse + specular) * attenuation;
    FragColor = vec4(result, 1.0);
}