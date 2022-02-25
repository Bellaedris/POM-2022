#pragma once
#include <string>
#include "Vector2.h"

/*! \class Circle2
* Representation of a Circle in a 2D space
*/
class Circle2
{
protected:
	Vector2 center; //!< The circle center
	double radius; //!< The circle radius

public:
	Circle2();
	Circle2(const Vector2& center, const double radius);
	Vector2 const GetCenter();
	double const GetRadius();
	void SetCenter(Vector2& center);
	void SetRadius(double radius);
	std::string const ToString();
	static bool const intersection(Circle2& cercle1, Circle2& cercle2);
	static bool const inside(Circle2& cercle, Vector2& point);
	void Translate(double x, double y);
};
