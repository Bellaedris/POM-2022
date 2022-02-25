#include "Circle2.h"
#include <iostream>
#include <string>
#include <stdlib.h> // for the abs

/*!
* Initializes a Circle2 with his radius at 0 and also his center
*/
Circle2::Circle2() : radius(0)
{
}
/*!
* Initializes a Vector2 with 2 parameters
* \param center a Vector2
* \param radius a double
*/
Circle2::Circle2(const Vector2& center, const double radius) : center(center), radius(radius)
{
}

/*!
* Gets the center of the Circle2
* \return the center component
*/
Vector2 const Circle2::GetCenter()
{
	return center;
}

/*!
* Gets the radius of the Circle2
* \return the radius component
*/
double const Circle2::GetRadius()
{
	return radius;
}

/*!
* Sets the center of the Circle2
* \param center a Vector2
*/
void Circle2::SetCenter(Vector2& center)
{
	this->center = center;
}

/*!
* Sets the radius of the Circle2
* \param radius a double
*/
void Circle2::SetRadius(double radius)
{
	this->radius = radius;
}

/*!
* \return a string value of the Circle2
*/
std::string const Circle2::ToString()
{
	return "centre:" + center.ToString() + ", radius:" + std::to_string(radius) + '\n';
}

/*!
* \return a boolean to know if two circle are intersected
* \param cercle1, cercle2 are Circle2
*/
bool const Circle2::intersection(Circle2& cercle1, Circle2& cercle2)
{
	return (Vector2::Distance(cercle1.center, cercle2.center) > abs(cercle1.radius - cercle2.radius) && Vector2::Distance(cercle1.center, cercle2.center) < (cercle1.radius + cercle2.radius));
}

/*!
* \return a center of the Circle2
* \param cercle a Circle2
* \param point a Vector2
*/
bool const Circle2::inside(Circle2& cercle, Vector2& point)
{
	return (Vector2::Distance(cercle.center, point) < cercle.radius);
}

/*!
* Translate the center of the circle
* \param x, y double
*/
void Circle2::Translate(double x, double y)
{
	center.SetX(center.GetX() + x);
	center.SetY(center.GetY() + y);
}

