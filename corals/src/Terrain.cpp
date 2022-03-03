#include "Terrain.h"
#include <iostream>
#include <fstream>
#include <set>
#include <vector>
#include <random>

/*!
* Initializes a terrain with its bounds, an heightmap and a number of points per axis
* Used to test terrains with already defined heightmaps
* \param bounds a box defining the borders of the terrain
* \param hm an already initialized heightmap
* \param nx the number of points on the x axis
* \param ny the number of points on the y axis
*/
Terrain::Terrain(Box2 bounds, QVector<double>& hm, int nx, int ny)
{
	this->bounds = bounds;
	this->heightmap = hm;
	this->nx = nx;
	this->ny = ny;
}

/*!
* Creates a terrain based on an image
* \param img a Qt image
* \param bounds the Box2 that defines the terrain
* \param minHeight the minimal height of the terrain
* \param maxHeight the maximal height of the terrain
*/
Terrain::Terrain(const QImage img, Box2 bounds, float minHeight, float maxHeight)
{
	this->bounds = bounds;
	this->nx = img.width();
	this->ny = img.height();
	this->minHeight = minHeight;
	this->maxHeight = maxHeight;
	this->heightmap = QVector<double>(this->nx * this->ny);

	if (img.isGrayscale() && !img.isNull())
	{
		//Qimg analysis. Analyse each point and give its color. 
		//give the current point a height based on the color of the point: 
		//our image is in greyscale, so all color channels will all be equals
		//the value returned will vary from 0 to 255, so we normalize it from 0 to 1
		// 0 for black, 1 for white, and lerp it inbetween
		for (int y = 0; y < this->ny; y++)
		{
			for (int x = 0; x < this->nx; x++)
			{
				float height = img.pixelColor(x, y).red();
				height = Normalize01(height, 0, 255);
				heightmap[Index(x, y)] = Lerp(minHeight, maxHeight, height);
			}
		}
	}
	else
	{
		std::cout << "Please provide a valid greyscale image" << std::endl;
	}
}

/*!
* Initializes a terrain with its bounds, initializes a heightmap and a number of points per axis
* \param bounds a box defining the borders of the terrain
* \param nx the number of points on the x axis
* \param ny the number of points on the y axis
* \param scale scale of the noise
* \param octave number of octave of the noise
* \param amplitude maximal value of the noise
* \param frequency frequency of the noise - higher values mean there is less space between two high points
* \param seed is generated automatically. If you specify it manually, you'll be able to reproduce a certain terrain.
*/
Terrain::Terrain(int nx, int ny, double scale, int octave, double amplitude, double frequency, unsigned seed = 0)
{
	this->bounds = Box2(Vector2(0,0), Vector2(nx / scale, ny / scale));
	this->scale = scale;
	this->octave = octave;
	this->amplitude = amplitude;
	this->frequency = frequency;
	this->nx = nx / scale;
	this->ny = ny / scale;
	this->heightmap = QVector<double>((this->nx) * (this->ny));
	this->maxHeight = amplitude;

	for (int i = 1; i <= octave; i++) {
		maxHeight += maxHeight / (2 * i);
	}

	noise::module::Perlin p;

	int i = 0;

	for (double y = 0; y < ny; y++) {
		for (double x = 0; x < nx; x++) {
			heightmap[i] = p.GetValue(x, y, 0);
			i++;
		}
	}

}

/*!
* Modifies the heightmap of the current terrain to apply debris flow erosion
* We create a map of all slopes because only using the slope value when updating
* the height will propagate incorrect values
* \param strength coefficient to increase or decrease the effects of erosion
*/
void Terrain::DebrisFlowErosion(float strength)
{
	std::vector<float> SLOPES(nx * ny);
	//creates a map of all slopes
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			heightmap[Index(x, y)] = Slope(x, y);
		}
	}

	//update the heightmap
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			float newHeight = Clamp(Height(x, y) - SLOPES.at(Index(x, y)) * strength, minHeight, maxHeight); //clamps the height to avoid negative values
			heightmap[Index(x, y)] = newHeight;
		}
	}
}

/*!
* Creates a path between 2 points, and edits the terrain to create a road
* \param i x coordinate of the beggining point
* \param j y coordinate of the beggining point
* \param k x coordinate of the end point
* \param l y coordinate of the end point
* \param roadSize size - radius - of the road
*/
void Terrain::CreateRoad(int i, int j, int k, int l, int roadSize) {
	int start = Index(i, j);
	int end = Index(k, l);

	std::vector<double> minDistance;
	std::vector<int> previous;
	DijkstraComputePaths(start, CreateAdjacencyList(), minDistance, previous);
	// commented lines are useful for debug
	//std::cout << "Distance from start to end: " << minDistance[end] << std::endl;
	std::vector<int> path = DijkstraGetShortestPathTo(end, previous);
	//std::cout << "Path : ";
	//std::copy(path.begin(), path.end(), std::ostream_iterator<int>(std::cout, " "));
	//std::cout << std::endl;

	std::vector<Vector3> V3Path = std::vector<Vector3>(nx * ny);
	//RoadFlattening(V3Path, 3); NOT USED YET - PERFORMANCES ISSUES
	for (int i = path.size() - 1; i > 0; i--) {
		int x = ReverseIndex(path[i]).GetX();
		int y = ReverseIndex(path[i]).GetY();
		Flatten(Circle2(ReverseIndex(path[i]), roadSize), Height(x, y), 10);
	}
}

/*!
* --- NOT USED YET ---
* Terraforms the input road. Only the points in the road radius will be modified.
* They should be modified so there is a smooth transition between the road and the sides.
* Also, the algorithm allows our points to be touched only once, compared to our current flattening method. 
* \param curve a vector containing all the points of the road
* \param size the road radius
*/
void Terrain::RoadFlattening(std::vector<Vector3> curve, int size) {
	for (int i = 0; i < nx; i++) {
		for (int j = 0; j < ny; j++) {
			double dz[2];
			distanceFromCurve(curve, Index(i,j), dz);
			dz[0] /= size; 
			if (dz[0] > 1) 
				continue; //don't touch the point if not in the road radius
			/*double a = pow( (1 - d * d), 3); //a in[0, 1]
			heightmap[Index(i, j)] = (1 - a) * Height(i, j) + a * dz[1];*/
		}
	}
}

/*!
* --- NOT USED YET - PERFORMANCES ISSUES ---
* Returns the shortest distance from p to one point of the curve stored in the result array.
* The initial array will be modified.
* \param curve the points that define the path
* \param p the point to test
* \param result the array that will contain the result
*/
void Terrain::distanceFromCurve(std::vector<Vector3> curve, int p, double result[])
{
	result[0] = 0;
	double distance; // distance from point to curve
	double prev; //previous distance for comparison
	int px = ReverseIndex(p).GetX();
	int py = ReverseIndex(p).GetY();
	for (int i = 0; i < curve.size(); i++) {
		prev = distance;
		distance = Vector3::Distance(curve.at(i), Position(px, py));
		if (distance < prev)
		{
			result[0] = distance;
			result[1] = Height(px, py);
		}
	}
}

/*!
* Creates a list that contains, for all points in the heightmap, 
* all their neighbors, and the cost to move to said neighbor.
* For more realistic roads, points not directly adjacent will be added if possible.
* Here are the points that will be added, with 1 being the point analysed, 
* 2 being its neighbors and 0 being the other points: \n
* O 2 O 2 O
* 2 2 2 2 2
* O 2 1 2 O
* 2 2 2 2 2
* O 2 O 2 O
* \return an list of adjacency that can be used by the Dijkstra implementation
*/
Terrain::adjacency_list_t Terrain::CreateAdjacencyList()
{
	adjacency_list_t adj_list(nx * ny);
	//adjacents when not on a border
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			int i = Index(x, y);
			if (x > 0)
				adj_list[i].push_back(neighbor(Index(x - 1, y), Cost(x, y, x - 1, y))); // left
			if (x > 0 && y < ny - 1)
				adj_list[i].push_back(neighbor(Index(x - 1, y + 1), Cost(x, y, x - 1, y + 1))); // bottom left
			if (y < ny - 1)
				adj_list[i].push_back(neighbor(Index(x, y + 1), Cost(x, y, x, y + 1))); // bottom
			if (y < ny - 1 && x < nx - 1)
				adj_list[i].push_back(neighbor(Index(x + 1, y + 1), Cost(x, y, x + 1, y + 1))); //bottom right
			if (x < nx - 1)
				adj_list[i].push_back(neighbor(Index(x + 1, y), Cost(x, y, x + 1, y))); // right
			if (x < nx - 1 && y > 0)
				adj_list[i].push_back(neighbor(Index(x + 1, y - 1), Cost(x, y, x + 1, y - 1))); // top right
			if (y > 0)
				adj_list[i].push_back(neighbor(Index(x, y - 1), Cost(x, y, x, y - 1))); // top
			if (x > 0 && y > 0)
				adj_list[i].push_back(neighbor(Index(x - 1, y - 1), Cost(x, y, x - 1, y - 1))); // top left
			//add more points to get smooth roads
			if (x > 1 && y > 0)
				adj_list[i].push_back(neighbor(Index(x - 2, y - 1), Cost(x, y, x - 2, y - 1))); 
			if (x > 0 && y > 1)
				adj_list[i].push_back(neighbor(Index(x - 1, y - 2), Cost(x, y, x - 1, y - 2)));
			if (x < nx - 1 && y > 1)
				adj_list[i].push_back(neighbor(Index(x + 1, y - 2), Cost(x, y, x + 1, y - 2)));
			if ( x < nx - 2 && y > 0)
				adj_list[i].push_back(neighbor(Index(x + 2, y - 1), Cost(x, y, x + 2, y - 1)));
			if (x > 1 && y < ny - 1)
				adj_list[i].push_back(neighbor(Index(x - 2, y + 1), Cost(x, y, x - 2, y + 1)));
			if (x > 0 && y < ny - 2)
				adj_list[i].push_back(neighbor(Index(x - 1, y + 2), Cost(x, y, x - 1, y + 2)));
			if (x < nx - 1 && y < ny - 2)
				adj_list[i].push_back(neighbor(Index(x + 1, y + 2), Cost(x, y, x + 1, y + 2)));
			if (x < nx - 2 && y < ny - 1)
				adj_list[i].push_back(neighbor(Index(x + 2, y + 1), Cost(x, y, x + 2, y + 1)));
		}
	}

	return adj_list;
}

/*!
* Computes the shortest path from one point to every other point in the heightmap
* Adapted from the code at https://rosettacode.org/wiki/Dijkstra%27s_algorithm#C.2B.2B
* \param source the starting point
* \param adjacency_list the adjacent points of all points of the map
* \param min_distance all the paths
* \param previous the previous point
*/
void Terrain::DijkstraComputePaths(int source,
	const adjacency_list_t& adjacency_list,
	std::vector<double>& min_distance,
	std::vector<int>& previous)
{
	int n = adjacency_list.size();
	min_distance.clear();
	min_distance.resize(n, max_weight);
	min_distance[source] = 0;
	previous.clear();
	previous.resize(n, -1);
	std::set<std::pair<double, int> > vertex_queue;
	vertex_queue.insert(std::make_pair(min_distance[source], source));
	
	while (!vertex_queue.empty())
	{
		double dist = vertex_queue.begin()->first;
		int u = vertex_queue.begin()->second;
		vertex_queue.erase(vertex_queue.begin());

		// Visit each edge exiting u
		const std::vector<neighbor>& neighbors = adjacency_list[u];
		for (std::vector<neighbor>::const_iterator neighbor_iter = neighbors.begin();
			neighbor_iter != neighbors.end();
			neighbor_iter++)
		{
			int v = neighbor_iter->target;
			double weight = neighbor_iter->weight;
			double distance_through_u = dist + weight;
			if (distance_through_u < min_distance[v]) {
				vertex_queue.erase(std::make_pair(min_distance[v], v));

				min_distance[v] = distance_through_u;
				previous[v] = u;
				vertex_queue.insert(std::make_pair(min_distance[v], v));

			}

		}
	}
}

/*!
* Get the shortest path to a point
* \param vertex the destination point
* \param previous all the shortest paths from a point to all the others
* previously computed using shortest path algorithm
* \return a vector containing the indexes of all the points composing the path
*/
std::vector<int> Terrain::DijkstraGetShortestPathTo(int vertex, const std::vector<int>& previous)
{
	std::vector<int> path;
	for (; vertex != -1; vertex = previous[vertex])
		path.push_back(vertex);
	return path;
}

/*
* Computes the cost to move from a point i j to 
* a point j k
* \param i x coordinate of the beggining point
* \param j y coordinate of the beggining point
* \param k x coordinate of the destination
* \param l y coordinate of the destination
*/
double Terrain::Cost(int i, int j, int k, int l)
{
	double d = (Position2D(i, j) - Position2D(k, l)).Length(); //distance
	double h = Height(i, j) - Height(k, l); // height difference

	double s = fabs(h / d); //absolute value of slope
	// the cost takes mostly the slope in account, then the distance
	// overall, it will mostly try to avoid changes of altitude before, then will get the shortest distance
	return 0.1 * d + 0.9 * s;
}

/*!
* Exports a terrain as a greyscale heightmap, png format.
* Places the image in the export folder
* \param filename of the exported file
*/
void Terrain::ExportTerrainAsHeightmap(std::string filename)
{
	QImage toExport = QImage(bounds.Width(), bounds.Height(), QImage::Format_Grayscale8);
	QString file = QString::fromStdString("exports/" + filename + ".jpg");

	//loop all pixels
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			int pixelColor = Normalize01(Height(x, y), minHeight, maxHeight) * 255;
			toExport.setPixelColor(x, y, QColor(pixelColor, pixelColor, pixelColor));
		}
	}
	//save img in the export folder
	toExport.save(file, nullptr, -1);
}

/*!
* Exports the terrain as an obj file in a "exports" folder
* Writes the points, then the normals, then links the points together
* \param filename name of the file
*/
void Terrain::ExportOBJ(std::string filename)
{
	std::ofstream exportFile("exports/" + filename + ".obj");

	if (exportFile)
	{
		WritePointsToOBJ(exportFile);
		WriteNormalsToOBJ(exportFile);
		WriteTrisToOBJ(exportFile);
	}
	else
	{
		//error when trying to open the file
		std::cout << "Error when trying to open the file" << std::endl;
	}
}

/*!
* Gets the height of the heightmap for the point located at x, y
* \param x the x coordinate
* \param y the y coordinate
* \return value of the Heightmap for the points x, y
*/
double Terrain::Height(int x, int y)
{
	return heightmap.at(Index(x, y));
}

/*!
* Gets the index in the heightmap of the point at the coordinates x, y
* \param x the x index of the point
* \param y the y index of the point
* \return index of the point in the heightmap
*/
int Terrain::Index(int x, int y)
{
	return nx * y + x;
}

/*!
* Get the coordinates x y of a point in the heightmap
* \param i the index of the point in the heightmap
*/
Vector2 Terrain::ReverseIndex(int i)
{
	return Vector2(i % nx, i / nx);
}

/*!
* Returns the position of a point
* \return A Vector3
*/
Vector3 Terrain::Position(int i, int j)
{
	Vector2 pxy = Position2D(i, j);
	return Vector3(pxy.GetX(), pxy.GetY(), Height(i, j));
}

/*!
* Returns the position of a point without the height
* \return A Vector2
*/
Vector2 Terrain::Position2D(int i, int j)
{
	i = Clamp(i, 0, nx);
	j = Clamp(j, 0, ny);

	Vector2 ab = bounds.Diagonal();
	Vector2 a = bounds.GetA();
	double posX = a.GetX() + ab.GetX() * double(i) / double(nx - 1.0);
	double posY = a.GetY() + ab.GetY() * double(j) / double(ny - 1.0);

	return Vector2(posX, posY);
}

/*!
* Calculates a gradient for a given point of the terrain
* The Gradient is a Vector representing the x variation when moving on y,
* and the y variation when moving on x
* \param x the x coordinate
* \param y the y coordinate
* \return The Gradient Vector
*/
Vector2 Terrain::Gradient(int x, int y)
{
	//our point mustn't be on the border of the terrain
	if (x > 0 && x < nx - 1 && y > 0 && y < ny - 1)
	{
		Vector2 a = bounds.GetA();
		Vector2 b = bounds.GetB();

		double ex = 2.0 * (a.GetX() - b.GetX()) / (nx - 1);
		double ey = 2.0 * (a.GetY() - b.GetY()) / (ny - 1);
		double gx = (Height(x + 1, y) - Height(x - 1, y)) / ex;
		double gy = (Height(x, y + 1) - Height(x, y - 1)) / ey;
		return Vector2(gx, gy);
	}
	else {
		return Vector2(0, 0);
	}
}

/*!
* Calculates a slope angle for a given point of the terrain
* \param x the x coordinate
* \param y the y coordinate
* \return a slope angle
*/
double Terrain::Slope(int& x, int& y)
{
	return Gradient(x, y).Length();
}

/*!
* Calculates a normal for a given point of the terrain
* \param x the x coordinate on nx
* \param y the y coordinate on ny
* \return a Vector 3 that describes the normal
*/
Vector3 Terrain::Normal(int x, int y)
{
	Vector2 grad = Gradient(x, y);
	return Vector3(-grad.GetX(), -grad.GetY(), 1) / Vector3(-grad.GetX(), -grad.GetY(), 1).Length();
}

/*!
* Writes all of the points coordinates to an obj file
* \param file an ofstream defining an OBJ file
*/
void Terrain::WritePointsToOBJ(std::ofstream& file)
{
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			//writes a line for each point that contains its coordinates on x, y and z
			file << "v " << Position(x, y).ToString() << std::endl;
		}
	}
}

/*!
* Writes the normals of all the points to an obj file
* \param file an ofstream defining an OBJ file
*/
void Terrain::WriteNormalsToOBJ(std::ofstream& file)
{
	for (int y = 0; y < ny; y++)
	{
		for (int x = 0; x < nx; x++)
		{
			//writes a line for each point that contains its coordinates on x, y and z
			file << "vn " << Normal(x, y).ToString() << std::endl;
		}
	}
}

/*!
* Writes all the tris necessary for the points to be all linked together to an OBJ file\n
* The logic to link our point is the following for the 9 points presented below:\n
* 1 2 3\n
* 4 5 6\n
* 7 8 9\n
* Any point must be linked in antichronological order. So 4 will
* be linked with 2 and 1, then with 5 and 1 to form 2 tris
* \param file an ofstream defining an OBJ file
*/
void Terrain::WriteTrisToOBJ(std::ofstream& file)
{
	//skips the first line as it has to points above 
	for (int y = 1; y < ny; y++)
	{
		//only write the point if he is not at the end of a line (as it would not have a point to its right)
		for (int x = 0; x < nx - 1; x++)
		{
			int current = Index(x,y) + 1; // the "number" of the current point in a "1D" fashion
			//write current, diagonal, above and current, right, diagonal
			int rDiagonal = current - (nx - 1); //the point in the right diagonal of the current
			int above = current - nx; //the point above the current
			int right = current + 1;//the point to the right of the current

			std::string lTri = std::to_string(current) + " " + std::to_string(rDiagonal) + " " + std::to_string(above);
			std::string rTri = std::to_string(current) + " " + std::to_string(right) + " " + std::to_string(rDiagonal);

			file << "f " << lTri << std::endl;
			file << "f " << rTri << std::endl;
		}
	}
}

/*!
* Normalizes a value in a range to a value between 0 and 1
* \param value the value to normalize
* \param min minimal value of the interval
* \param max maximal value of the interval
* \return value normalized between 0 and 1
*/
double Terrain::Normalize01(double value, double min, double max)
{
	return (value - min) / (max - min);
}

/*!
* Clamps a value between a min and a max
* \param value the value to clamp
* \param min the lowest possible value
* \param max the highest possible value
* \return min if value is below min, max if above max, value otherwise
*/
double Terrain::Clamp(double value, double min, double max)
{
	if (value < min)
	{
		return min;
	}
	else if (value > max)
	{
		return max;
	}
	else
	{
		return value;
	}
}

/*!
* Linear interpolation between 2 values
* \param a start value, returned when t = 0
* \param b end value, returned when t = 1
* \param t value to interpolate between a and b
* \return t linearly interpolated between a and b
*/
double Terrain::Lerp(double a, double b, double t)
{
	return a + (b - a) * t;
}

/*!
* Make a place in the terrain totally flat.
* \param c cirlce where the terrain will be flat
* \param h height of the terrain flatten
* \param t additionnal area so the flat terrain and the other terrain can gather smothly
*/
void Terrain::Flatten(Circle2 c, double h, double t) {
	Vector2 v2 = c.GetCenter();
	Circle2 c2(v2, c.GetRadius() + t);
	for (int y = v2.GetY() - c2.GetRadius(); y < v2.GetY() + c2.GetRadius(); y++) {
		for (int x = v2.GetX() - c2.GetRadius(); x < v2.GetX() + c2.GetRadius(); x++) {
			if (x > 0 && x < nx && y>0 && y < ny) {
				Vector2 v(x, y);
				if (Circle2::inside(c, v)) {
					heightmap[Index(x, y)] = h;
				}
				else {
					if (Circle2::inside(c2, v)) {
						double distance = Vector2::Distance(v2, c2.GetCenter());
						if (h < heightmap[Index(x, y)]) {
							heightmap[Index(x, y)] = heightmap[Index(x, y)] - (heightmap[Index(x, y)] - h) * Crater(distance, c.GetRadius(), c2.GetRadius());
						}
						else {
							heightmap[Index(x, y)] = heightmap[Index(x, y)] + (h - heightmap[Index(x, y)]) * Crater(distance, c.GetRadius(), c2.GetRadius());
						}
					}
				}
			}
		}
	}
}
/*!
* fill your heightmap with a perlin noise algorithm
*/
void Terrain::Heightmap_Perlin() {

	//properties for th FBM
	float lacunarity = 2.0;
	float gain = 0.4;
	// Initial values
	float ancien_amplitude = this->amplitude;
	float ancien_frequency = this->frequency;
	int iterator = 0;

	for (double y = 0; y < ny; y++) {
		for (double x = 0; x < nx; x++) {
			// the FBM algorithm which overlay perlin noise
			for (int i = 0; i < octave; i++) {
				this->heightmap.replace(iterator, this->heightmap.at(iterator) + (Perlin_Noise((y * scale) * frequency, (x * scale) *  frequency)) * amplitude);
				frequency *= lacunarity;
				amplitude *= gain;
			}
			iterator++;
			amplitude = ancien_amplitude;
			frequency = ancien_frequency;
		}
	}
	
	
}
/*!
* the perlin noise algorithm for one value
* \param x  the x value of my point
* \param y  the y value of my point
* \return height_point the height of my point in x,y wich follow my perlin noise
*/
double Terrain::Perlin_Noise(double x, double y) {

	double gradient2[4][2] = {{1,1},{1,-1},{-1,1},{-1,-1} };
	int x0 = (int)x;
	int y0 = (int)y;
	int x1 = x0+1;
	int y1 = y0+1;
	
	//we do a masking, ii and jj are between 0 and 255
	int ii = y0 & 255;
	int jj = x0 & 255;


	// the first step of our algorithm we determine the 4 vector bond to our point
	int grad1 = permtable[permtable[ii] + jj];
	int grad2 = permtable[permtable[ii + 1] + jj];
	int grad3 = permtable[permtable[ii] + jj + 1];
	int grad4 = permtable[permtable[ii + 1] + jj + 1];

	int indiceX_grad1 = (grad1 & 1) ? 1 : -1;
	int indiceY_grad1 = (grad1 & 2) ? 1 : -1;
	Vector2* vecteurPremier = new Vector2(indiceX_grad1, indiceY_grad1);
	int indiceX_grad2 = (grad2 & 1) ? 1 : -1;
	int indiceY_grad2 = (grad2 & 2) ? 1 : -1;
	Vector2* vecteurDeuxieme = new Vector2(indiceX_grad2, indiceY_grad2);
	int indiceX_grad3 = (grad3 & 1) ? 1 : -1;
	int indiceY_grad3 = (grad3 & 2) ? 1 : -1;
	Vector2* vecteurTroisieme = new Vector2(indiceX_grad3, indiceY_grad3);
	int indiceX_grad4 = (grad4 & 1) ? 1 : -1;
	int indiceY_grad4 = (grad4 & 2) ? 1 : -1;
	Vector2* vecteurDernier = new Vector2(indiceX_grad4, indiceY_grad4);

	double tempX = x - (int)x;
	double tempY = y - (int)y;
	Vector2* temp = new Vector2(tempX, tempY);
	
	//second step of our algorithm we calculate the influence of these 4 vector to our point

	double temp_s = Vector2::Dot_ortogonale(*vecteurPremier,  Vector2(tempX, tempY));

	double temp_t = Vector2::Dot_ortogonale(*vecteurTroisieme, Vector2(tempX - 1, tempY));

	double temp_u = Vector2::Dot_ortogonale(*vecteurDeuxieme, Vector2(tempX, tempY - 1));

	double temp_v = Vector2::Dot_ortogonale(*vecteurDernier,  Vector2(tempX - 1, tempY - 1));
	
	//we do the coefficient of interpolation of our x value

	double Sx = 3 * (x - x0) * (x - x0) - 2 * (x - x0) * (x - x0) * (x - x0);

	//we smooth the value 2 to 2
	double stLisse = temp_s + Sx * (temp_t - temp_s);
	double uvLisse = temp_u + Sx * (temp_v - temp_u);

	//we do the coefficient of interpolation of our y value

	double Sy = 3 * (y - y0) * (y - y0) - 2 * (y - y0) * (y - y0) * (y - y0);

	//we calculate the final height of our point
	double height_point = stLisse + Sy * (uvLisse - stLisse);
	height_point = (height_point + 1) / 2;
	return height_point;
}

/*!
* Returns a value between 0 and 1, the more the point is near ri, the higher the value will be
* \param d position of the point
* \param ri position of the point of the inside circle
* \param re position of the point of the outside circle
*/
double Terrain::Crater(double d, double ri, double re) {
	if (d < ri) return 1;
	if (d > ri) return 0;
	else {
		double u = (d - ri) / (re - ri);
		return pow((1 - pow(u, 2)), 2);
	}
}

/*int main()
{
	// Tests some terrain generation from images, then from perlin.
	
	auto start = std::chrono::high_resolution_clock::now();

	QImage img = QImage("resources/test_heightmap.jpg");
	Box2 bounds = Box2(Vector2(0, 0), Vector2(img.width(), img.height()));
	Terrain t4 = Terrain(img, bounds, 0, 100);
	t4.ExportTerrainAsHeightmap("fromImg");
	auto tick = std::chrono::high_resolution_clock::now();
	std::cout << "Image import: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;
	
	start = std::chrono::high_resolution_clock::now();
	Terrain t = Terrain(10, 10, 0.01, 5, 100, 0.5);
	t.ExportTerrainAsHeightmap("tPerlinMountains");
	//t.ExportOBJ("tPerlinMountains");
	tick = std::chrono::high_resolution_clock::now();
	std::cout << "Moutain terrain: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;
	start = std::chrono::high_resolution_clock::now();
	t.DebrisFlowErosion(1);
	t.ExportTerrainAsHeightmap("tPerlinErodedMountains");
	tick = std::chrono::high_resolution_clock::now();
	std::cout << "Erosion time Moutain terrain: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;
	
	start = std::chrono::high_resolution_clock::now();
	Terrain t3 = Terrain(10, 10, 0.01, 5, 30, 0.5);
	t3.CreateRoad(0, 500, 999, 500, 1);
	t3.CreateRoad(0, 0, 999, 999, 1);
	t3.ExportTerrainAsHeightmap("tPerlinMountainsRoad");
	tick = std::chrono::high_resolution_clock::now();
	std::cout << "Plains Moutain with road: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;

	start = std::chrono::high_resolution_clock::now();
	t3.ExportOBJ("tPerlinMountainRoad");
	tick = std::chrono::high_resolution_clock::now();
	std::cout << "Export of a 1000x1000 terrain: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;

	start = std::chrono::high_resolution_clock::now();
	Terrain t2 = Terrain(10, 10, 0.01, 5, 30, 0.2);
	t2.ExportTerrainAsHeightmap("tPerlinPlains");
	tick = std::chrono::high_resolution_clock::now();
	std::cout << "Plains terrain: " << std::chrono::duration_cast<std::chrono::milliseconds>(tick - start).count() << std::endl;

	return 1;
}*/