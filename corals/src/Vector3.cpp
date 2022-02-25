#include <iostream>
#include <string>
#include <math.h>
#include "Vector3.h"

using namespace std;

/*!
 *Initializes a Vector3 with 0 for each coordinates
*/
Vector3::Vector3()
{
	x = y = z = 0;
}

/*!
*Initializes a Vector3 with 3 parameters
*\param x the coordinates on x
*\param y the coordinates on y
*\param z the coordinates on z
*/
Vector3::Vector3(float x, float y, float z)
{
	this->x = x;
	this->y = y;
	this->z = z;
}

/*!
*Gets the x value of the Vector3
*\return the x component
*/
double Vector3::GetX() const
{
	return x;
}

/*!
*Gets the y value of the Vector3
*\return the y component
*/
double Vector3::GetY() const
{
	return y;
}

/*!
*Gets the z value of the Vector3
*\return the z component
*/
double Vector3::GetZ() const
{
	return z;
}

/*!
*Returns a string representation of the Vector3
*\return a string of form: x y z
*/
string const Vector3::ToString()
{
	return to_string(x) + " " + to_string(y) + " " + to_string(z);
}

/*!
*Calculates the length of a Vector3
*\return the length of the vector
*/
double const Vector3::Length()
{
	return sqrt(x * x + y * y + z * z);
}

/*!
*Distance between two Vector3 a and b
*\param a the first point
*\param b the second point
*\return The distance between the points a and b
*/
double const Vector3::Distance(const Vector3& a, const Vector3& b)
{
	return (a - b).Length();
}

/*!
*Checks equality between two vectors
*\return true if the 2 vector are strictly identical, false otherwise
*/
bool const Vector3::Equals(Vector3& a, Vector3& b)
{
	return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
}

/*!
*Calculates the dot product of two Vector3
*\param a the first Vector3
*\param b the second Vector3
*\return the dot product of the 2 Vector3 as a double
*/
double const Vector3::Dot(Vector3& a, Vector3& b)
{
	return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
}

/*!
*Calculates the cross product of two Vector3
*\param a the first Vector3
*\param b the second Vector3
*\return the cross product of the 2 Vector3 as a double
*/
Vector3 const Vector3::Cross(Vector3& a, Vector3& b)
{
	float crossX = a.GetY() * b.GetZ() - a.GetZ() * b.GetY();
	float crossY = a.GetX() * b.GetZ() - a.GetZ() * b.GetX();
	float crossZ = a.GetX() * b.GetY() - a.GetY() * b.GetX();
	Vector3 cross = Vector3(crossX, crossY, crossZ);
	//return the normalized vector
	return cross.Length() == 0 ? cross : cross / cross.Length();
}

/*!
 *Brief addition
 *\param a, b 2 Vector3
 */
Vector3 operator+ (const Vector3& a, const Vector3& b)
{
	return Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
};

/*!
 *Brief substraction
 *\param a, b 2 Vector3
 */
Vector3 operator- (const Vector3& a, const Vector3& b)
{
	return Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
};

/*!
 *Multiplication with a number
 *\param a a Vector3
 *\param b a double
 *\return the Vector multiplied by the double
 */
Vector3 operator* (const Vector3& a, const double& b)
{
	return Vector3(a.x * b, a.y * b, a.z * b);
};

/*!
 *Multiplication with a number
 *\param a a double
 *\param b a Vector3
 *\return the Vector multiplied by the double
 */
Vector3 operator* (const double& a, const Vector3& b)
{
	return Vector3(b.x * a, b.y * a, b.z * a);
};

/*!
 *Division with a number
 *\param a a Vector3
 *\param b a double
 *\return the Vector divided by the double
 */
Vector3 operator/ (const Vector3& a, const double& b)
{
	return Vector3(a.x / b, a.y / b, a.z / b);
};

/*!
 *Division with a number
 *\param a a double
 *\param b a Vector3
 *\return the Vector divided by the double
 */
Vector3 operator/ (const double& a, const Vector3& b)
{
	return Vector3(b.x / a, b.y / a, b.z / a);
};