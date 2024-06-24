#pragma once

#include <glad/glad.h> // include glad to get all the required OpenGL headers

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include <string>
#include <unordered_map>

class Shader
{
public:
    // the program ID
    unsigned int ID;

    // constructor reads and builds the shader
    Shader(const std::string& vertexPath, const std::string& fragmentPath) : Shader(vertexPath.c_str(), fragmentPath.c_str()) {}
    Shader(const char* vertexPath, const char* fragmentPath);
    ~Shader() { glDeleteProgram(ID); }
    // use/activate the shader
    void use() { glUseProgram(ID); }
    // utility uniform functions
    void setBool(const std::string& name, bool value) const { glUniform1i(getUniformLocation(name), (int)value); }
    // ------------------------------------------------------------------------
    void setInt(const std::string& name, int value) const { glUniform1i(getUniformLocation(name), value); }
    // ------------------------------------------------------------------------
    void setFloat(const std::string& name, float value) const { glUniform1f(getUniformLocation(name), value); }

    void setMatrix4(const std::string& name, GLfloat* value) const { glUniformMatrix4fv(getUniformLocation(name), 1, GL_FALSE, value); }
    void setMatrix3(const std::string& name, GLfloat* value) const { glUniformMatrix3fv(getUniformLocation(name), 1, GL_FALSE, value); }

    void setVec3(const std::string& name, const glm::vec3& value) const { glUniform3fv(getUniformLocation(name), 1, glm::value_ptr(value)); }
    void setVec3(const std::string& name, float x, float y, float z) const { glUniform3f(getUniformLocation(name), x, y, z); }

private:
    void checkCompileErrors(unsigned int shader, std::string type);
    GLint getUniformLocation(const std::string& name) const
    {
        auto search = cachedUniformLocations.find(name);
        if (search == cachedUniformLocations.end())
        {
            cachedUniformLocations.insert({ name, glGetUniformLocation(ID, name.c_str()) });
        }
        return cachedUniformLocations[name];
    }

    mutable std::unordered_map<std::string, GLint> cachedUniformLocations;
};