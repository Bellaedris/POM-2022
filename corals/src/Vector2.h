#pragma once
#include <string>

/*! \class Vector2
* Representation of a Vector in 3 dimensions x and y
*/
class Vector2
{
protected:
	double x, y; //!< The vector coordinates on the x and y axis

public:
	Vector2();
	Vector2(double x, double y);
	double GetX() const;
	double GetY() const;
	void SetX(double x);
	void SetY(double y);
	std::string const ToString();
	double const Length();
	static double const Distance(Vector2 a, Vector2 b);
	static double const Dot(Vector2& a, Vector2& b);
	static double const Dot_ortogonale(const Vector2& a, const Vector2& b);
	//TODO: Angle(Vector2 a, Vector2 b): angle between the 2 vectors
	static bool const Equals(Vector2& a, Vector2& b);

	//operators overlaod
	friend Vector2 operator+ (const Vector2& a, const Vector2& b); //vector addition
	friend Vector2 operator- (const Vector2& a, const Vector2& b); //vector substraction
	friend Vector2 operator* (const Vector2& a, const double& b); // multiply a vector by a number
	friend Vector2 operator* (const double& a, const Vector2& b); // multiply a vector by a number
	friend Vector2 operator/ (const Vector2& a, const double& b); // divide a vector by a number
	friend Vector2 operator/ (const double& a, const Vector2& b); // multiply a vector by a number
};
