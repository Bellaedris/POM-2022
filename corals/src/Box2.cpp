#include "Box2.h"
#include "Vector2.h"
#include "Circle2.h"

/*!
* Initializes a Box 2 with 0 for each coordinates
*/
Box2::Box2()
{
	a = Vector2(0, 0);
	b = Vector2(0, 0);
}

/*!
* Initializes a Box2 with 2 parameters
* \param a the Vector of the a point
* \param b the Vector of the b point
*/
Box2::Box2(const Vector2& a, const Vector2& b)
{
	this->a = a;
	this->b = b;
}

/*!
* Checks if a point is inside the box or not
* \param p the position of the point
*/
bool const Box2::Inside(Vector2& p)
{
	return (p.GetX() > a.GetX()) && (p.GetX() < b.GetX()) && (p.GetY() > a.GetY()) && (p.GetY() < b.GetY());
}


/*!
* Gets a
* \return the point a of the box
*/
Vector2 const Box2::GetA()
{
	return this->a;
}

/*!
* Gets b
* \return the point b of the box
*/
Vector2 const Box2::GetB()
{
	return this->b;
}

/*!
* Checks if a point of the circle is inside the box or not
* \param c the circle
*/
bool const Box2::Intersection(Circle2& c)
{
	return false; //stub
}

/*!
* Calculates the length of the box
* \return the length of the box
*/
float const Box2::Width()
{
	return b.GetX() - a.GetX();
}

/*!
* Calculates the height of the box
* \return the height of the box
*/
float const Box2::Height()
{
	return b.GetY() - a.GetY();
}

/*!
* Calculates the Diagonal Vector of the box
* \return The diagonal of the box as a Vector2
*/
Vector2 Box2::Diagonal() const {
	return this->b - this->a;
}