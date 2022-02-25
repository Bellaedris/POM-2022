#pragma once
#include <QVector>
#include <QImage>
#include <vector>
#include "Box2.h"
#include "Vector2.h"
#include "Vector3.h"

/*! \class Terrain
* Representation of a 3 dimensions terrain. It can be constructed from scratch using noise, or using an image. 
* It can be exported to an OBJ file to be opened on any 3D software compatible with OBJ files
*/
class Terrain {

protected:
	QVector<double> heightmap; //!< A vector containing the height value for each point in our terrain
	Box2 bounds; //!< Box that represent our terrain
	int nx, ny; //!< The number of points for x and y
	int maxHeight = 100; //!< Maximal value of the heightmap. Default is 100
	int minHeight = 0; //!< Minimal value of the heightmap. Default is 0
	double* gradient; //!< Gradient used for FBM
	int octave; //!< Number of octaves used for FBM
	double amplitude; //!< Amplitude used for FBM
	double frequency; //!< Frequency used for FBM
	int permtable[511]; //!< table of permutations used with Perlin noise
	double scale; //!< the scale of our terrain wich follow perlin noise

	//Shortest path variables
	const double max_weight = std::numeric_limits<double>::infinity(); //!< Max weight of a transition from one point to another
	struct neighbor {
		int target;
		double weight;
		neighbor(int arg_target, double arg_weight)
			: target(arg_target), weight(arg_weight) { }
	}; //!< Describes a neighbor, it has a target and a weight to go to the target
	typedef std::vector<std::vector<neighbor> > adjacency_list_t; //!< Graph structure containing all the points and their neighbors
	
	void WritePointsToOBJ(std::ofstream& file);
	void WriteNormalsToOBJ(std::ofstream& file);
	void WriteTrisToOBJ(std::ofstream& file);
	static double Normalize01(double value, double min, double max);
	static double Clamp(double value, double min, double max);
	static double Lerp(double a, double b, double t);

public:
	Terrain(Box2 bounds, QVector<double>& hm, int nx, int ny);
	Terrain(Box2 bounds, QVector<float>& hm, int nx, int ny);
	Terrain(const QImage img, Box2 bounds, float minHeight, float maxHeight);
	Terrain(int nx, int ny, double frequence, int octave, double amplitude, double frequency, unsigned seed);
	void ExportOBJ(std::string filename);
	double Height(int x, int y);
	int Index(int x, int y);
	Vector2 ReverseIndex(int i);
	Vector3 Position(int i, int j);
	Vector2 Position2D(int i, int j);
	Vector2 Gradient(int x, int y);
	double Slope(int& x, int& y);
	Vector3 Normal(int x, int y);
	void ExportTerrainAsHeightmap(std::string filename);
	void DebrisFlowErosion(float strength);
	void Flatten(Circle2 c, double h, double t);
	double Crater(double d, double ri, double re);
	void DijkstraComputePaths(int source, const adjacency_list_t& adjacency_list, std::vector<double> &minDistance, std::vector<int> &previous);
	std::vector<int> DijkstraGetShortestPathTo(int vertex, const std::vector<int>& previous);
	double Cost(int i, int j, int k, int l);
	adjacency_list_t CreateAdjacencyList();
	void CreateRoad(int i, int j, int k, int l, int roadSize);
	void Heightmap_Perlin();
	double Perlin_Noise(double x, double y);
	void RoadFlattening(std::vector<Vector3> curve, int size);
	void distanceFromCurve(std::vector<Vector3> curve, int p, double result[]);
	double R(const Vector2& p) const;
};