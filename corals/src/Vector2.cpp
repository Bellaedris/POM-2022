#include <iostream>
#include <string>
#include <math.h>
#include "Vector2.h"

using namespace std;

/*!
 * Initializes a Vector 2 with 0 for each coordinates
*/
Vector2::Vector2()
{
	x = y = 0;
}

/*!
* Initializes a Vector2 with a x and a y
* \param x the coordinates on x
* \param y the coordinates on y
*/
Vector2::Vector2(double x, double y)
{
	this->x = x;
	this->y = y;
}

/*!
* Gets the x value of the Vector2
* \return the x component
*/
double Vector2::GetX() const
{
	return x;
}

/*!
* Gets the y value of the Vector2
* \return the y component
*/
double Vector2::GetY() const
{
	return y;
}

/*!
* Sets the  x value of the Vector2
* \param x the new x
*/
void Vector2::SetX(double x)
{
	this->x = x;
}

/*!
* Sets the y value of the Vector2
* \param y the new y
*/
void Vector2::SetY(double y)
{
	this->y = y;
}

/*!
* Returns a string representation of the Vector2
* \return a string of the form: x y
*/
string const Vector2::ToString()
{
	return to_string(x) + " " + to_string(y);
}

/*!
* Calculates the length (norm) of a vector
* \return the length of the vector
*/
double const Vector2::Length()
{
	return sqrt(x * x + y * y);
}

/*!
* Distance between a and b
* \param a the first point
* \param b the second point
* \return The distance between the points a and b
*/
double const Vector2::Distance(Vector2 a, Vector2 b)
{
	return (a - b).Length();
}

/*!
* Checks equality between two vectors
* \return true if the 2 vector are strictly identical on x and y, false otherwise
*/
bool const Vector2::Equals(Vector2& a, Vector2& b)
{
	return (a.x == b.x) && (a.y == b.y);
}

/*!
* Calculates the dot product of two Vector2
* \param a the first Vector2
* \param b the second Vector2
* \return the dot product of the 2 Vector2 as a double
*/
double const Vector2::Dot_ortogonale(const Vector2& a, const Vector2& b)
{
	return (a.x * b.x) + (a.y * b.y);
}

double const Vector2::Dot(Vector2& a, Vector2& b)
{
	return a.Length() * b.Length();
}

/*!
 *Brief addition
 *\param a, b 2 vectors
 */
Vector2 operator+ (const Vector2& a, const Vector2& b)
{
	return Vector2(a.x + b.x, a.y + b.y);
};

/*!
 *Brief substraction
 *\param a, b 2 vectors
 */
Vector2 operator- (const Vector2& a, const Vector2& b)
{
	return Vector2(a.x - b.x, a.y - b.y);
};

/*!
 *Multiplication with a number
 *\param a a Vector2
 *\param b a double
 *\return the Vector multiplied by the double
 */
Vector2 operator* (const Vector2& a, const double& b)
{
	return Vector2(a.x * b, a.y * b);
};

/*!
 *Multiplication with a number
 *\param a a double
 *\param b a Vector2
 *\return the Vector multiplied by the double
 */
Vector2 operator* (const double& a, const Vector2& b)
{
	return Vector2(b.x * a, b.y * a);
};

/*!
 *Division with a number
 *\param a a Vector2
 *\param b a double
 *\return the Vector divided by the double
 */
Vector2 operator/ (const Vector2& a, const double& b)
{
	return Vector2(a.x / b, a.y / b);
};

/*!
 *Division with a number
 *\param a a double
 *\param b a Vector2
 *\return the Vector divided by the double
 */
Vector2 operator/ (const double& a, const Vector2& b)
{
	return Vector2(b.x / a, b.y / a);
};