#include "Terrain.h"

int main() {
    Terrain t = Terrain(10, 10, 0.01, 5, 100, 0.5, 0);
	//t.ExportOBJ("tPerlinMountains");
	t.ExportTerrainAsHeightmap("tPerlin");
	return 0;
}