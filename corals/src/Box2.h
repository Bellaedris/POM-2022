#pragma once
#include "Vector2.h"
#include "Circle2.h"

/*! \class Box2
* Representation of a Box in a 2D space
*/
class Box2
{
protected:
	Vector2 a, b; //!< The coordinates of the 2 diagonals of the box

public:
	Box2();
	Box2(const Vector2 &a, const Vector2 &b);
	bool const Inside(Vector2 &p);
	Vector2 const GetA();
	Vector2 const GetB();
	float const Width();
	float const Height();
	Vector2 Diagonal() const;
	bool const Intersection(Circle2& c);
};
