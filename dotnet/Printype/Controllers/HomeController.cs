using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Printype.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View(Attendees.Data);
		}
		public ActionResult Label(string code)
		{
			var attendee = Attendees.Data[code];
			var labeller = new Labeller();
			var label = labeller.Draw(attendee);
			MemoryStream ms = new MemoryStream();
			label.Save(ms, ImageFormat.Png);
			ms.Position = 0;
			return new FileStreamResult(ms, "image/png");
		}
	}

	public class Labeller
	{
		public Bitmap Draw(Attendee attendee)
		{
			var result = new Bitmap(700, 300);
			using (var graphics = Graphics.FromImage(result))
			{
				graphics.FillRectangle(Brushes.White, 0, 0, 700, 300);
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
				Font font = new Font("Arial", 100);
				SizeF size;
				do
				{
					font = new Font("Arial", font.Size - 10);
					size = graphics.MeasureString(attendee.FirstName, font);
				} while (size.Width > 600 || size.Height > 150);
				graphics.DrawString(attendee.FirstName, font, Brushes.Black, new Point(10, 10), StringFormat.GenericTypographic);

				font = new Font("Arial", 100);
				do
				{
					font = new Font("Arial", font.Size - 10);
					size = graphics.MeasureString(attendee.FullName, font);
				} while (size.Width > 500 || size.Height > 100);

				graphics.DrawString(attendee.FullName, font, Brushes.Black, new Point(10, 165));
				font = new Font("Arial", 100);
				do
				{
					font = new Font("Arial", font.Size - 10);
					size = graphics.MeasureString(attendee.Event, font);
				} while (size.Width > 500 || size.Height > 100);

				graphics.DrawString(attendee.Event, font, Brushes.Black, new Point(10, 220));

				using (var qr = DrawQRCode(attendee))
				{
					graphics.DrawImage(qr, 500, 100);
				}
			}
			return (result);
		}
		public Bitmap DrawQRCode(Attendee attendee)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(attendee.FirstName + " " + attendee.LastName + " / " + attendee.Event, QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			Bitmap qrCodeImage = qrCode.GetGraphic(5);
			return (qrCodeImage);
		}
	}

	public static class Attendees
	{
		private static Dictionary<string, Attendee> attendees = new Dictionary<string, Attendee>()
		{
			{ "aa-aa-aa", new Attendee { FirstName = "Arthur", LastName = "Aardvark", Event = "Angular Meetup Group"} },
			{ "bb-bb-bb", new Attendee { FirstName = "Barbara", LastName = "Bannerman", Event = "BDD Exchange"} },
			{ "cc-cc-cc", new Attendee { FirstName = "Cynthia", LastName = "Cornflower", Event = "C++ University"} },
			{ "dd-dd-dd", new Attendee { FirstName = "Don", LastName = "Davis", Event = "DDDX"} },
			{ "nn-nn-nn", new Attendee { FirstName = "Nathalie", LastName = "Christmann-Cooper", Event = "The Long Names Meetup Group"} }
		};

		public static Dictionary<string, Attendee> Data {
			get {
				return (attendees);
			}
		}
	}
	public class Attendee
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Event { get; set; }
		public DateTime? PrintedAt { get; set; }
		public string FullName { get { return FirstName + " " + LastName; } }
	}

}
