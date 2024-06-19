#pragma once

#include <glad/glad.h> // include glad to get all the required OpenGL headers

#include <string>
#include <unordered_map>

class Shader
{
public:
    // the program ID
    unsigned int ID;

    // constructor reads and builds the shader
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