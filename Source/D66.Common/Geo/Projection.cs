using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D66.Common.Mathmatics;

namespace D66.Common.Geo
{


	/// <summary>
	/// Implements a RijksCoordinaten Driehoeksstelsel projection
	/// </summary>
	public class RijksDriehoeksProjection : Projection
	{
		public override LatLng ToLatLng(Vector2 p)
		{
			double lat, lon;
			RijksDriehoeksStelsel.XYToLongLat(new Vector2(p.X, p.Y), out lon, out lat);
			return new LatLng(lat, lon);
		}

		public override Vector2 FromLatLng(LatLng l)
		{
			var vect = RijksDriehoeksStelsel.LongLatToXY(l.Longitude, l.Latitude);
			return new Vector2(Math.Round(vect.X, 1), Math.Round(vect.Y, 1));
		}
	}


	/// <summary>
	/// Deze klasse bevalt methoden om te converteren tussen lengte- en breedtegraden en 
	/// coördinaten in het rijksdriehoeksstelsel. Er wordt gebruik gemaakt van een reeks-
	/// ontwikkeling met een nauwkeurigheid van ca. 1m
	/// </summary>
	public static class RijksDriehoeksStelsel
	{

		#region Coëfficienten benadering

		private static double a01 = 3236.0331637;
		private static double a20 = -32.5915821;
		private static double a02 = -.2472814;
		private static double a21 = -.8501341;
		private static double a03 = -.0655238;
		private static double a22 = -.0171137;
		private static double a40 = .0052771;
		private static double a23 = -.0003859;
		private static double a41 = .0003314;
		private static double a04 = .0000371;
		private static double a42 = .0000143;
		private static double a24 = -.0000090;
		private static double b10 = 5261.3028966;
		private static double b11 = 105.9780241;
		private static double b12 = 2.4576469;
		private static double b30 = -.8192156;
		private static double b31 = -.0560092;
		private static double b13 = .0560089;
		private static double b32 = -.0025614;
		private static double b14 = .0012770;
		private static double b50 = .0002574;
		private static double b33 = -.0000973;
		private static double b51 = .0000293;
		private static double b15 = .0000291;
		private static double c01 = 190066.98903;
		private static double c11 = -11830.85831;
		private static double c21 = -114.19754;
		private static double c03 = -32.38360;
		private static double c31 = -2.34078;
		private static double c13 = -.60639;
		private static double c23 = .15774;
		private static double c41 = -.04158;
		private static double c05 = -.00661;
		private static double d10 = 309020.31810;
		private static double d02 = 3638.36193;
		private static double d12 = -157.95222;
		private static double d20 = 72.97141;
		private static double d30 = 59.79734;
		private static double d22 = -6.43481;
		private static double d04 = .09351;
		private static double d32 = -.07379;
		private static double d14 = -.05419;
		private static double d40 = -.03444;

		#endregion

		/// <summary>
		/// Converteert (x, y) in rijksdriehoekscoordinaten naar lengte- en breedte graden
		/// </summary>
		/// <param name="v"></param>
		/// <param name="longitude"></param>
		/// <param name="latitude"></param>
		public static void XYToLongLat(Vector2 v, out double longitude, out double latitude)
		{
			Vector2 t = (v - Center) / 100000.0;
			double dx = t.X;
			double dx2 = dx * dx;
			double dx3 = dx2 * dx;
			double dx4 = dx2 * dx2;
			double dx5 = dx3 * dx2;
			double dy = t.Y;
			double dy2 = dy * dy;
			double dy3 = dy2 * dy;
			double dy4 = dy2 * dy2;
			double dy5 = dy3 * dy2;
			double d_lambda =
				b10 * dx + b11 * dx * dy + b30 * dx3 + b12 * dx * dy2 + b31 * dx3 * dy + b13 * dx * dy3 +
				b50 * dx5 + b32 * dx3 * dy2 + b14 * dx * dy4 + b51 * dx5 * dy + b15 * dx * dy5;
			double d_phi =
				a01 * dy + a20 * dx2 + a02 * dy2 + a21 * dx2 * dy + a03 * dy3 + a40 * dx4 +
				a22 * dx2 * dy2 + a41 * dx4 * dy + a23 * dx2 * dy3 + a42 * dx4 * dy2 + a24 * dx2 * dy4;


			longitude = (d_lambda / 3600.0) + RadToDeg(Lambda0);
			latitude = (d_phi / 3600.0) + RadToDeg(Phi0);
		}

		/// <summary>
		/// Converteert lengte- en breedtegraden naar (x, y) coordinaten in het rijksdriehoeksstelsel
		/// </summary>
		/// <param name="longitude"></param>
		/// <param name="latitude"></param>
		/// <returns></returns>
		public static Vector2 LongLatToXY(double longitude, double latitude)
		{
			double dl = (longitude - RadToDeg(Lambda0)) * 3600.0 / 10000.0;
			double dl2 = dl * dl;
			double dl3 = dl2 * dl;
			double dl4 = dl2 * dl2;
			double dl5 = dl3 * dl2;
			double dp = (latitude - RadToDeg(Phi0)) * 3600.0 / 10000.0;
			double dp2 = dp * dp;
			double dp3 = dp2 * dp;
			double dp4 = dp2 * dp2;
			double dp5 = dp3 * dp2;
			double dx =
				c01 * dl + c11 * dp * dl + c21 * dp2 * dl + c31 * dp3 * dl + c13 * dp * dl3 +
				c23 * dp2 * dl3 + c41 * dp4 * dl + c05 * dl5;
			double dy =
				d10 * dp + d20 * dp2 + d02 * dl2 + d12 * dp * dl2 + d30 * dp3 + d22 * dp2 * dl2 +
				d40 * dp4 + d04 * dl4 + d32 * dp3 + dl2 + d14 * dl * dl4;
			return new Vector2(X0 + dx, Y0 + dy);
		}

		/// <summary>
		/// Converteert radialen naar graden
		/// </summary>
		/// <param name="rad"></param>
		/// <returns></returns>
		private static double RadToDeg(double rad)
		{
			return rad * 180.0 / Math.PI;
		}

		/// <summary>
		/// Converteert graden naar radialen
		/// </summary>
		/// <param name="deg"></param>
		/// <returns></returns>
		private static double DegToRad(double deg)
		{
			return deg * Math.PI / 180.0;
		}



		// De coordinaten van de Onze Lieven Vrouwentoren in Amersfoort
		public static readonly double Phi0 = DegToRad(52.1551722222222);
		public static readonly double Lambda0 = DegToRad(5.38720277777778);

		// Oorsprong van het rijkscoördinatenstelsel
		public static readonly double X0 = 155000;
		public static readonly double Y0 = 463000;
		public static Vector2 Center = new Vector2(X0, Y0);

	}

	/// <summary>
	/// Contains 
	/// </summary>
	public abstract class Projection
	{

		public abstract LatLng ToLatLng(Vector2 p);

		public abstract Vector2 FromLatLng(LatLng l);



	}



	/// <summary>
	/// Lat is always between -90 and 90.
	/// Long is always between -180 and 180.
	/// </summary>
	public struct LatLng
	{


		public LatLng(double lat, double lng)
		{
			_Latitude = lat;
			_Longitude = lng;
			Latitude = lat;
			Longitude = lng;
		}

		/// <summary>
		/// Latitude. positive = north of equator, negative = south
		/// </summary>
		public double Latitude
		{
			get { return _Latitude; }
			set
			{
				if (value < -90 || value > 90)
				{
					throw new ArgumentException("Invalid latitude: " + value);
				}
				_Latitude = value;
			}
		}
		private double _Latitude;

		/// <summary>
		/// Returns the latitude in radians
		/// </summary>
		public double LatitudeRadians
		{
			get
			{
				return (Latitude * Math.PI) / 180.0;
			}
		}

		/// <summary>
		/// Longitude. Positive = east, negative = west
		/// </summary>
		public double Longitude
		{
			get { return _Longitude; }
			set
			{
				if (value < -180 || value > 180)
				{
					throw new ArgumentException("Invalid longitude: " + value);
				}
				_Longitude = value;
			}
		}
		private double _Longitude;


		/// <summary>
		/// Returns the longitude in radians
		/// </summary>
		public double LongitudeRadians
		{
			get
			{
				return (Longitude * Math.PI) / 180.0;
			}
		}

		public override string ToString()
		{
			return string.Format("({0:0.000000}, {1:0.000000})", Latitude, Longitude);
		}



		public static LatLng Max(IEnumerable<LatLng> items)
		{
			double? lat = null, lon = null;
			foreach (var l in items)
			{
				if (!lat.HasValue || l.Latitude > lat)
				{
					lat = l.Latitude;
				}
				if (!lon.HasValue || l.Longitude > lon)
				{
					lon = l.Longitude;
				}
			}
			if (!lat.HasValue)
			{
				return new LatLng();
			}
			return new LatLng(lat.Value, lon.Value);
		}

		public static LatLng Min(IEnumerable<LatLng> items)
		{
			double? lat = null, lon = null;
			foreach (var l in items)
			{
				if (!lat.HasValue || l.Latitude < lat)
				{
					lat = l.Latitude;
				}
				if (!lon.HasValue || l.Longitude < lon)
				{
					lon = l.Longitude;
				}
			}
			if (!lat.HasValue)
			{
				return new LatLng();
			}
			return new LatLng(lat.Value, lon.Value);
		}
	}

}
