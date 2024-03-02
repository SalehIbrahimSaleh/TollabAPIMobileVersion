using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TollabAPI.Services
{
    public interface IZoomManagerService
    {
        (HttpStatusCode status, string hostURL, string joinURL, long meetingId, string meetingPassword) CreateSchedualMeeting(string name, int duration, DateTime date, long countryId);
        HttpStatusCode Delete(long meetingId);
        HttpStatusCode UpdateSchedualMeeting(string name, int duration, DateTime date, long countryId, long meetingId);
    }
}
