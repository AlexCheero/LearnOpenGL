#pragma once

#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>



class Camera
{
	const float nearPlane = 0.1f;
	const float farPlane = 100.0f;
	const float sensitivity = 1.0f;
	const float pitchBound = 89.0f;
	const float minFov = 30.0f;
	const float maxFov = 90.0f;
	const float cameraSpeed = 10.0f;

	const glm::vec3 WorldUp{ 0.0f, 1.0f, 0.0f };

	//TODO: extract into transform component
	glm::vec3 cameraPos{ 0.0f, 0.0f, 3.0f };
	glm::vec3 cameraFront{ 0.0f, 0.0f, -1.0f };
	glm::vec3 cameraUp{ 0.0f, 1.0f, 0.0f };

	float yaw = -90.0f;
	float pitch = 0.0f;
	float fov = 45.0f;

public:
	glm::vec3 CameraPos() { return cameraPos; }
	glm::vec3 CameraFront() { return cameraFront; }

	Camera() : cameraPos(0.0f, 0.0f, 3.0f), cameraFront(0.0f, 0.0f, -1.0f), cameraUp(0.0f, 1.0f, 0.0f), yaw(-90.0f), pitch(0.0f), fov(45.0f)
	{}

	glm::mat4 GetViewMatrix() { return glm::lookAt(cameraPos, cameraPos + cameraFront, cameraUp); }
	glm::mat4 GetPerspectiveMatrix(float ratio) { return glm::perspective(glm::radians(fov), ratio, nearPlane, farPlane); }

	void Rotate(float xDelta, float yDelta, float deltaTime);
	void SetFov(float offset) { fov = glm::clamp(fov - offset, minFov, maxFov); }
	void Move(glm::vec3 dir, float deltaTime, bool normalizeDir = true);
	glm::vec3 GetCameraRight() { return glm::normalize(glm::cross(cameraFront, WorldUp)); }
	//not used yet
	glm::vec3 GetCameraUp() { return glm::normalize(glm::cross(GetCameraRight(), cameraFront)); }
};