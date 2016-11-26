using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;
using GoogleMaps.LocationServices;
using Geolocation;
using Shorthand.Geodesy;
using Geocoding;
using Geocoding.Google;
using System.Device.Location;

namespace WeFix.Logic
{

    public class T
    {
        public int DepartmentId { get; set; }
        public double Distance { get; set; }
    }
    public class Coordinates
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
    public static class CoordinatesDistanceExtensions
    {
        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
        }

        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }
    }
    public class UnitOfLength
    {
        public static UnitOfLength Kilometers = new UnitOfLength(1.609344);
        public static UnitOfLength NauticalMiles = new UnitOfLength(0.8684);
        public static UnitOfLength Miles = new UnitOfLength(1);

        private readonly double _fromMilesFactor;

        private UnitOfLength(double fromMilesFactor)
        {
            _fromMilesFactor = fromMilesFactor;
        }

        public double ConvertFromMiles(double input)
        {
            return input * _fromMilesFactor;
        }
    }

    public class Helpers
    {
        private const string API_KEY = "AIzaSyC0ggfMf91HoVhnrIQNuvrOUc-74JZFMPY";
        private const string baseUrl = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&key={2}";

        public string GetAddressFromLatLong(double Lat, double Long)
        {
            IGeocoder geocoder = new GoogleGeocoder() { ApiKey = API_KEY };
            Location loc = new Location(Lat, Long);
            IEnumerable<Address> coordinates = geocoder.ReverseGeocode(loc);
            return coordinates.First().FormattedAddress;
        }



        /// <summary>
        /// Function Returns List of Faults that are relevant to the Organisation with relation to jurisdiction and category
        /// </summary>
        /// <param name="db">Context For access to database</param>
        /// <param name="organisation">The organisation concerned</param>
        /// <param name="allFaults">List of All Faults</param>
        /// <returns>IEnumerable of faults relevant to the organisation</returns>
        public IEnumerable<Fault> GetRelevantFaults(EFDbContext db, Organisation organisation, List<Fault> allFaults, bool resolved)
        {
            try
            {

                //Declare list that will be populated
                List<Fault> relFaults = new List<Fault>();
                //Get all departments owned by the organisation
                List<Department> departments = (from x in db.Departments where x.OrganisationId == organisation.OrganisationId select x).ToList();
                //iterate through departments
                foreach (var dept in departments)
                {
                    //iterate through faults
                    foreach (var f in allFaults)
                    {
                        if (resolved == false)
                        {
                            if (f.Resolved == false)
                            {
                                //check if categories match
                                if (dept.CategoryId == f.CategoryId)
                                {
                                    decimal distance = 0;
                                    if (f.Latitude.ToString().Contains(","))
                                    {
                                        var lat1 = Convert.ToDouble(dept.Latitude);
                                        var lng1 = Convert.ToDouble(dept.Longitude);
                                        var lat2 = Convert.ToDouble(f.Latitude);
                                        var lng2 = Convert.ToDouble(f.Longitude);
                                        distance = Convert.ToDecimal(new Coordinates(lat1, lng1)
                                               .DistanceTo(
                                                   new Coordinates(lat2, lng2),
                                                   UnitOfLength.Kilometers
                                               ));
                                    }
                                    else
                                    {
                                        distance = Convert.ToDecimal(new Coordinates(dept.Latitude, dept.Longitude)
                                              .DistanceTo(
                                                  new Coordinates(f.Latitude, f.Longitude),
                                                  UnitOfLength.Kilometers
                                              ));
                                    }

                                    //check if department is within its specified jurisdiction
                                    if (distance <= dept.Jurisdiction)
                                    {
                                        if (f.Resolved == resolved)
                                        {
                                            //Add to fault
                                            relFaults.Add(f);
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (f.Resolved)
                            {
                                //check if categories match
                                if (dept.CategoryId == f.CategoryId)
                                {
                                    decimal distance = 0;
                                    if (f.Latitude.ToString().Contains(","))
                                    {
                                        var lat1 = Convert.ToDouble(dept.Latitude);
                                        var lng1 = Convert.ToDouble(dept.Longitude);
                                        var lat2 = Convert.ToDouble(f.Latitude);
                                        var lng2 = Convert.ToDouble(f.Longitude);
                                        distance = Convert.ToDecimal(new Coordinates(lat1, lng1)
                                               .DistanceTo(
                                                   new Coordinates(lat2, lng2),
                                                   UnitOfLength.Kilometers
                                               ));
                                    }
                                    else
                                    {
                                        distance = Convert.ToDecimal(new Coordinates(dept.Latitude, dept.Longitude)
                                              .DistanceTo(
                                                  new Coordinates(f.Latitude, f.Longitude),
                                                  UnitOfLength.Kilometers
                                              ));
                                    }

                                    //check if department is within its specified jurisdiction
                                    if (distance <= dept.Jurisdiction)
                                    {
                                        if (f.Resolved == resolved)
                                        {
                                            //Add to fault
                                            relFaults.Add(f);
                                        }

                                    }
                                }
                            }
                        }

                    }
                }
                //Convert to IEnumerablef for the list page
                IEnumerable<Fault> RelFaults = relFaults as IEnumerable<Fault>;
                //return list of faults
                return RelFaults;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Get Suburb from LatLng value
        /// </summary>
        /// <param name="lat">Latitude double Value</param>
        /// <param name="lng">Longitude double Value</param>
        /// <returns></returns>
        public string GetSuburb(double lat, double lng)
        {
            string latString, lngString;
            //Ensure that latlng value decimal is a period and not a comma
            if (lat.ToString().Contains(","))
            {
                latString = lat.ToString().Replace(",", ".");
                lngString = lng.ToString().Replace(",", ".");
            }
            else
            {
                latString = lat.ToString();
                lngString = lng.ToString();
            }

            //Declare the address of the reverse geocode xml file to be loaded
            string xmlString = string.Format(baseUrl, latString, lngString, API_KEY);
            //Declare the variables that we will be using from the Xml file

            string suburb_1 = string.Empty;
            string formatted_address = string.Empty;
            string type = string.Empty;
            //Declare an instance of an Xml Doc
            XmlDocument xmlDoc = new XmlDocument();
            //Load the Xml
            xmlDoc.Load(xmlString);
            //Get the status node
            var nodeStatus = xmlDoc.SelectSingleNode("//GeocodeResponse/status");
            //check if status is okay
            if (nodeStatus.InnerText == "OK")
            {
                //Get a list of all address component nodes
                var nodeList = xmlDoc.SelectNodes("//GeocodeResponse/result");
                //There are over 100 diferrent nodes to iterate through. This will severely slow down
                //the system. So to counter this. We have a while loop to stop the foreach loop when
                //the required "suburb" variable to populated. Since the result we want is only in the 4th
                //node, this will reduce the work load significantly

                //Iterate through the list
                while (suburb_1 == string.Empty)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        try
                        {
                            //get the long name
                            formatted_address = node["formatted_address"].InnerText;
                            //The type we are interested in is the third type declaration. So skip two siblings
                            //to get to the type we want. Not every node will have a third sibling. Hence the use
                            //of a try catch to acount for this and sibling return null
                            //Once we finally find the correct type, grab it and populate the suburb variable

                            if (suburb_1 == string.Empty)
                            {
                                suburb_1 = formatted_address.Split(',')[1].TrimStart();
                            }
                        }
                        catch
                        {
                            suburb_1 = string.Empty;
                        }
                    }
                    //If Suburb is found, loop ends
                }



            }
            //return the Suburb           
            return suburb_1;

        }

        public string GetAddress(double lat, double lng)
        {
            string latString, lngString;
            //Ensure that latlng value decimal is a period and not a comma
            if (lat.ToString().Contains(","))
            {
                latString = lat.ToString().Replace(",", ".");
                lngString = lng.ToString().Replace(",", ".");
            }
            else
            {
                latString = lat.ToString();
                lngString = lng.ToString();
            }

            //Declare the address of the reverse geocode xml file to be loaded
            string xmlString = string.Format(baseUrl, latString, lngString, API_KEY);
            //Declare the variables that we will be using from the Xml file

            string formatted_address = string.Empty;
            string type = string.Empty;
            //Declare an instance of an Xml Doc
            XmlDocument xmlDoc = new XmlDocument();
            //Load the Xml
            xmlDoc.Load(xmlString);
            //Get the status node
            var nodeStatus = xmlDoc.SelectSingleNode("//GeocodeResponse/status");
            //check if status is okay
            if (nodeStatus.InnerText == "OK")
            {
                //Get a list of all address component nodes
                var nodeList = xmlDoc.SelectNodes("//GeocodeResponse/result");
                //There are over 100 diferrent nodes to iterate through. This will severely slow down
                //the system. So to counter this. We have a while loop to stop the foreach loop when
                //the required "suburb" variable to populated. Since the result we want is only in the 4th
                //node, this will reduce the work load significantly

                //Iterate through the list
                while (formatted_address == string.Empty)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        try
                        {
                            //get the long name
                            formatted_address = node["formatted_address"].InnerText;
                            //The type we are interested in is the third type declaration. So skip two siblings
                            //to get to the type we want. Not every node will have a third sibling. Hence the use
                            //of a try catch to acount for this and sibling return null
                            //Once we finally find the correct type, grab it and populate the suburb variable

                        }
                        catch
                        {
                            formatted_address = string.Empty;
                        }
                    }
                    //If Suburb is found, loop ends
                }



            }
            //return the Suburb           
            return formatted_address;

        }


    }

    public class EmailSettings
    {
        public string MailToAddress = "################@gmail.com";
        public string MailFromAddress = "#################@gmail.com";
        public bool UseSsl = true;
        public string Username = "################@gmail.com";
        public string Password = "##############";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\wefix_emails";
    }
    public class Email
    {
        private EmailSettings emailSettings = new EmailSettings();

        public void ProcessContact(string from, string subject, string body)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                    emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                MailMessage mailMessage = new MailMessage(
                emailSettings.MailFromAddress, //From
                emailSettings.MailToAddress, //To
                "Contact Us: " + subject, //Subject
                "Message from: " + from + Environment.NewLine + body); // Body
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
        
        public void ProcesFault(Fault fault, EFDbContext db)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                var pubUser = (from x in db.PublicUsers where x.PublicUserId == fault.PublicUserId select x).FirstOrDefault();
                var user = (from x in db.Users where x.Id == pubUser.UserId select x).FirstOrDefault();
                var Category = (from x in db.Categories where x.CategoryId == fault.CategoryId select x).FirstOrDefault();
                var subCat = "none";
                if (fault.SubCategoryId != null)
                {
                    subCat = (from x in db.SubCategories where x.SubCategoryId == fault.SubCategoryId select x.Name).FirstOrDefault();
                }
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                    emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder();
                body.Append("<html><head><style>.post{border-bottom:1px solid #d2d6de;margin-bottom:15px;padding-bottom:15px;color:#666}.post:last-of-type{border-bottom:0;margin-bottom:0;padding-bottom:0}.post .user-block{margin-bottom:15px}.user-block:before,.user-block:after{content:' ';display:table}.user-block:after{clear:both}.user-block img{width:40px;height:40px;float:left}.user-block .username,.user-block .description,.user-block .comment{display:block;margin-left:50px}.user-block .username{font-size:16px;font-weight:600}.user-block .description{color:#999;font-size:13px}.user-block.user-block-sm .username,.user-block.user-block-sm .description,.user-block.user-block-sm .comment{margin-left:40px}.user-block.user-block-sm .username{font-size:14px}.img-sm,.img-md,.img-lg,.box-comments .box-comment img,.user-block.user-block-sm img{float:left}.img-sm,.box-comments .box-comment img,.user-block.user-block-sm img{width:30px!important;height:30px!important}.img-circle{border-radius:50%}.img-bordered-sm{border:2px solid #d2d6de;padding:2px}.box-comments .username{color:#444;display:block;font-weight:600}.widget-user .widget-user-username{margin-top:0;margin-bottom:5px;font-size:25px;font-weight:300;text-shadow:0 1px 1px rgba(0,0,0,0.2)}.widget-user-2 .widget-user-username{margin-top:5px;margin-bottom:5px;font-size:25px;font-weight:300}.widget-user-2 .widget-user-username,.widget-user-2 .widget-user-desc{margin-left:75px}.profile-username{font-size:21px;margin-top:5px}.user-block .username,.user-block .description,.user-block .comment{display:block;margin-left:50px}.user-block .username{font-size:16px;font-weight:600}.user-block .description{color:#999;font-size:13px}.user-block.user-block-sm .username,.user-block.user-block-sm .description,.user-block.user-block-sm .comment{margin-left:40px}.user-block.user-block-sm .username{font-size:14px}.box-comments .username{color:#444;display:block;font-weight:600}.user-block .username,.user-block .description,.user-block .comment{display:block;margin-left:50px}.user-block .description{color:#999;font-size:13px}.user-block.user-block-sm .username,.user-block.user-block-sm .description,.user-block.user-block-sm .comment{margin-left:40px}.list-inline{padding-left:0;margin-left:-5px;list-style:none}.list-inline > li{display:inline-block;padding-right:5px;padding-left:5px}.link-black{color:#666}.link-black:hover,.link-black:focus{color:#999}</style><h1>A new fault has been submitted</h1></head><body>");
                body.Append("</hr>");
                body.Append("<div> <div class='post'> <div class='user-block'> <img class='img-circle img-bordered-sm' src='");
                body.Append("http://localhost:46777/" + fault.ImageURL.Replace("~", ""));
                body.Append("' alt='user image' /> <span class='username'> <a href='#'> ");
                body.Append(fault.FaultId.ToString());
                body.Append("</a> </span> <span class='description'><strong>Date</strong> ");
                body.Append(fault.DateCreated.ToString());
                body.Append("</span><span class='description'><strong>Category</strong> ");
                body.Append(Category.Name);
                body.Append("</span><span class='description'><strong>Sub-Category</strong> ");
                body.Append(subCat);
                body.Append("</span><span class='description'><strong>Severity</strong> ");
                body.Append(fault.SeverityId.ToString());
                body.Append("</p><p><strong>Description:</strong> ");
                body.Append(fault.Description);
                body.Append("</p><p><strong>LatLng:</strong> ");
                body.Append(fault.Latitude.ToString() + " || " + fault.Longitude.ToString());
                body.Append("</p><ul class='list-inline'><li><a href='http://localhost:47000/Faults/DetailsForPublic/?' class='link-black text-sm'><i class='fa fa-arrow-circle-left margin-r-5'></i> Go Online</a></li></ul></div></div></body></html>");

                MailMessage mailMessage = new MailMessage(
                emailSettings.MailFromAddress, //From
                emailSettings.MailToAddress, //To
                "New Fault submitted!", //Subject
                body.ToString()); // Body
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
        public void ProcessOrganisation(Organisation org, EFDbContext db)
        {
            var orgManager = (from x in db.OrganisationManagers where x.OrganisationId == org.OrganisationId select x).FirstOrDefault();
            var user = db.Users.Find(orgManager.UserId);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                    emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder();
                body.Append("<html><head><link rel='stylesheet' href='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'></head><body><h4>Organisation Registration</h4><hr/><p>");
                body.Append(user.FirstName + " " + user.LastName);
                body.Append(" has requested to register ");
                body.Append(org.Name);
                body.Append(", to confirm please <a href='http://localhost:46777/Organisations/ConfirmOrganisation/" + org.OrganisationId + "'>click here</a></p>");
                body.Append(" <dl class='dl-horizontal'><dt>Email:</dt><dd>");
                body.Append(user.Email);
                body.Append("</dd><dt>Cell:</dt><dd>");
                body.Append(user.Cell);
                body.Append("</dd></dl></body></html>");
                MailMessage mailMessage = new MailMessage(
                emailSettings.MailFromAddress, //From
                emailSettings.MailToAddress, //To
                "New Organisation Registration Request!", //Subject
                body.ToString()); // Body
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
        public void ProcessOrgConfirmation(Organisation org, EFDbContext db)
        {
            var orgManager = (from x in db.OrganisationManagers where x.OrganisationId == org.OrganisationId select x).FirstOrDefault();
            var user = db.Users.Find(orgManager.UserId);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                    emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder();
                body.Append("<html><head><link rel='stylesheet' href='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'></head><body><link rel='stylesheet' href='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'><h4>WeFix Organisation Registration Confirmed</h4><hr/><p>");

                body.Append("Thank you for registering your organisation, ");
                body.Append(org.Name);
                body.Append(", to start managing your WeFix Account please <a href='http://localhost:46777/Organisations/Dashboard/" + org.OrganisationId + "'>click here</a></p>");
                body.Append("</body></html>");
                MailMessage mailMessage = new MailMessage(
                emailSettings.MailFromAddress, //From
                user.Email, //To
                "Organisation Registration Confirmed!", //Subject
                body.ToString()); // Body
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}
