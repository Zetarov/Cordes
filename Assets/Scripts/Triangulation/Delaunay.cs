using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DelaunayVoronoi
{
    public class DelaunayTriangulator
    {
        private float MaxX { get; set; }
        private float MaxY { get; set; }
        private IEnumerable<Triangle> border;

        public IEnumerable<Point> GeneratePoints(int amount, float maxX, float maxY)
        {
            MaxX = maxX;
            MaxY = maxY;

            // TODO make more beautiful
            var point0 = new Point(0, 0);
            var point1 = new Point(0, MaxY);
            var point2 = new Point(MaxX, MaxY);
            var point3 = new Point(MaxX, 0);
            var points = new List<Point>() { point0, point1, point2, point3 };
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            var random = new Random();
            for (int i = 0; i < amount - 4; i++)
            {
                var pointX = (float)random.NextDouble() * MaxX;
                var pointY = (float)random.NextDouble() * MaxY;
                points.Add(new Point(pointX, pointY));
            }

            return points;
        }

        public void GenerateBorder(IEnumerable<Point> points)
        {
            if(points.Count() == 0)
            {
                return;
            }

            float minX = points.First().X;
            float maxX = minX;
            float minY = points.First().Y;
            float maxY = minY;

            foreach(Point point in points)
            {
                if(point.X < minX)
                {
                    minX = point.X;
                }
                else if(point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }
                else if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            var point0 = new Point(minX, minY);
            var point1 = new Point(minX, maxY);
            var point2 = new Point(maxX, maxY);
            var point3 = new Point(maxX, minY);

            UnityEngine.Debug.Log($"{minX} {maxX} | {minY} {maxY}");

            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };
        }

        public IEnumerable<Triangle> BowyerWatson(IEnumerable<Point> points)
        {
            //var supraTriangle = GenerateSupraTriangle();
            var triangulation = new HashSet<Triangle>(border);

            foreach (var point in points)
            {
                var badTriangles = FindBadTriangles(point, triangulation);
                var polygon = FindHoleBoundaries(badTriangles);

                foreach (var triangle in badTriangles)
                {
                    foreach (var vertex in triangle.Vertices)
                    {
                        vertex.AdjacentTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach (var edge in polygon.Where(possibleEdge => possibleEdge.Point1 != point && possibleEdge.Point2 != point))
                {
                    var triangle = new Triangle(point, edge.Point1, edge.Point2);
                    triangulation.Add(triangle);
                }
            }

            //triangulation.RemoveWhere(o => o.Vertices.Any(v => supraTriangle.Vertices.Contains(v)));
            return triangulation;
        }

        private List<Edge> FindHoleBoundaries(ISet<Triangle> badTriangles)
        {
            var edges = new List<Edge>();
            foreach (var triangle in badTriangles)
            {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            var grouped = edges.GroupBy(o => o);
            var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
            return boundaryEdges.ToList();
        }

        private Triangle GenerateSupraTriangle()
        {
            //   1  -> maxX
            //  / \
            // 2---3
            // |
            // v maxY
            var margin = 500f;
            var point1 = new Point(0.5f * MaxX, -2f * MaxX - margin);
            var point2 = new Point(-2f * MaxY - margin, 2f * MaxY + margin);
            var point3 = new Point(2f * MaxX + MaxY + margin, 2f * MaxY + margin);
            return new Triangle(point1, point2, point3);
        }

        private ISet<Triangle> FindBadTriangles(Point point, HashSet<Triangle> triangles)
        {
            var badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
            return new HashSet<Triangle>(badTriangles);
        }
    }
}