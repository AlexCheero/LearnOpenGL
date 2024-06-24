#include "Camera.hpp"

void Camera::Rotate(float xDelta, float yDelta, float deltaTime)
{
	float xoffset = xDelta * deltaTime * sensitivity;
	float yoffset = yDelta * deltaTime * sensitivity;

	yaw += xoffset;
	pitch = glm::clamp(pitch + yoffset, -pitchBound, pitchBound);

	float sinYaw = sin(glm::radians(yaw));
	float cosYaw = cos(glm::radians(yaw));
	float sinPitch = sin(glm::radians(pitch));
	float cosPitch = cos(glm::radians(pitch));

	glm::vec3 dir;
	dir.x = cosYaw * cosPitch;
	dir.y = sinPitch;
	dir.z = sinYaw * cosPitch;

	cameraFront = glm::normalize(dir);
}

void Camera::Move(glm::vec2 dir, float deltaTime, bool normalizeDir /*= true*/)
{
	if (dir.x == 0 && dir.y == 0)
		return;

	if (normalizeDir)
		dir = glm::normalize(dir);
	float cameraSpeedDelta = deltaTime * cameraSpeed;
	cameraPos += cameraSpeedDelta * (dir.y * cameraFront + dir.x * GetCameraRight());
}
