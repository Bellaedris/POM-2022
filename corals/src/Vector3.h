#pragma once
#include <string>

/*! \class Vector3
* Representation of a Vector in 3 dimensions x, y and z
*/
class Vector3
{
protected:
	double x, y, z; //!< The vector coordinates on the x, y and z axis

public:
	Vector3();
	Vector3(float x, float y, float z);
	double GetX() const;
	double GetY() const;
	double GetZ() const;
	std::string const ToString();
	double const Length();
	static double const Distance(const Vector3& a, const Vector3& b);
	static double const Dot(Vector3& a, Vector3& b);
	static Vector3 const Cross(Vector3& a, Vector3& b);
	//TODO: Angle(Vector3 a, Vector3 b): angle between the 2 vectors
	static bool const Equals(Vector3& a, Vector3& b);

	//operators overlaod
	friend Vector3 operator+ (const Vector3& a, const Vector3& b); //vector addition
	friend Vector3 operator- (const Vector3& a, const Vector3& b); //vector substraction
	friend Vector3 operator* (const Vector3& a, const double& b); // multiply a vector by a number
	friend Vector3 operator* (const double& a, const Vector3& b); // multiply a vector by a number
	friend Vector3 operator/ (const Vector3& a, const double& b); // divide a vector by a number
	friend Vector3 operator/ (const double& a, const Vector3& b); // multiply a vector by a number
};
