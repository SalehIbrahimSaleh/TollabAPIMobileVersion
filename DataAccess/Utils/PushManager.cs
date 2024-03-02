using DataAccess.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
namespace DataAccess.Utils
{
    public class PushManager
    {
        public static void PushToStudentToIphoneDevice(String token, string message,long? NotificationToId,long? ReferenceId,long? ContentId,long? VideoQuestionId, float? Minute = 0)
        {
            try
            {
                string cerPath = ConfigurationManager.AppSettings["iosStudentCer"];
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string cerPassword = ConfigurationManager.AppSettings["iosStudentPassword"];
                string fullPath = basePath + cerPath;

                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, fullPath, cerPassword);


                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);

                apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                    aggregateEx.Handle(ex => {

                        // See what kind of exception it was to further diagnose
                        if (ex is GcmNotificationException)
                        {
                            var notificationException = (GcmNotificationException)ex;

                            // Deal with the failed notification
                            var gcmNotification = notificationException.Notification;
                            var description = notificationException.Description;

                            Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                        }
                        else if (ex is GcmMulticastResultException)
                        {
                            var multicastException = (GcmMulticastResultException)ex;

                            foreach (var succeededNotification in multicastException.Succeeded)
                            {
                                Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                            }

                            foreach (var failedKvp in multicastException.Failed)
                            {
                                var n = failedKvp.Key;
                                var e = failedKvp.Value;

                                Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                            }

                        }
                        else if (ex is DeviceSubscriptionExpiredException)
                        {
                            var expiredException = (DeviceSubscriptionExpiredException)ex;

                            var oldId = expiredException.OldSubscriptionId;
                            var newId = expiredException.NewSubscriptionId;

                            Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                            if (!string.IsNullOrWhiteSpace(newId))
                            {
                                // If this value isn't null, our subscription changed and we should update our database
                                Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                            }
                        }
                        else if (ex is RetryAfterException)
                        {
                            var retryException = (RetryAfterException)ex;
                            // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                            Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                        }
                        else
                        {
                            Console.WriteLine("GCM Notification Failed for some unknown reason");
                        }

                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("Apple Notification Sent!");
                };

                // Start the broker
                apnsBroker.Start();

                // var modelToJson = JsonConvert.SerializeObject(technicianNotification);

                string title = "Tollab";
                string payloadobject = "{\"aps\" :"
                + " {\"alert\" : {\"title\" : \"" + title + "\",\"body\" :\"" + message + "\"},"
                + "\"badge\" : 1" + ", \"sound\":\"default\"}," + "\"NotificationToId\" : " + NotificationToId + "" + "," + "\"ContentId\" : " + ContentId + "" + "," + "\"Minute\" : " + Minute + "" + "," + "\"VideoQuestionId\" : " + VideoQuestionId + "" + "," + "\"ReferenceId\" : " + ReferenceId + " }";
                var Payloadd = JObject.Parse(payloadobject);
                // Queue a notification to send
                apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = token,
                    Payload = Payloadd
                });

                // Stop the broker, wait for it to finish   
                // This isn't done after every message, but after you're
                // done with the broker
                apnsBroker.Stop();

            }
            catch (Exception )
            {

            }

        }
        public static void PushToTeacherToIphoneDevice(String token, string message, long? NotificationToId, long? ReferenceId, long? ContentId,long? VideoQuestionId, float? Minute = 0)
        {
            try
            {
                string cerPath = ConfigurationManager.AppSettings["iosTeacherCer"];
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string cerPassword = ConfigurationManager.AppSettings["iosTeacherPassword"];
                string fullPath = basePath + cerPath;

                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, fullPath, cerPassword);


                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);

                apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {

                    aggregateEx.Handle(ex =>
                    {

                        // See what kind of exception it was to further diagnose
                        if (ex is GcmNotificationException)
                        {
                            var notificationException = (GcmNotificationException)ex;

                            // Deal with the failed notification
                            var gcmNotification = notificationException.Notification;
                            var description = notificationException.Description;

                            Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                        }
                        else if (ex is GcmMulticastResultException)
                        {
                            var multicastException = (GcmMulticastResultException)ex;

                            foreach (var succeededNotification in multicastException.Succeeded)
                            {
                                Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                            }

                            foreach (var failedKvp in multicastException.Failed)
                            {
                                var n = failedKvp.Key;
                                var e = failedKvp.Value;

                                Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                            }

                        }
                        else if (ex is DeviceSubscriptionExpiredException)
                        {
                            var expiredException = (DeviceSubscriptionExpiredException)ex;

                            var oldId = expiredException.OldSubscriptionId;
                            var newId = expiredException.NewSubscriptionId;

                            Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                            if (!string.IsNullOrWhiteSpace(newId))
                            {
                                // If this value isn't null, our subscription changed and we should update our database
                                Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                            }
                        }
                        else if (ex is RetryAfterException)
                        {
                            var retryException = (RetryAfterException)ex;
                            // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                            Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                        }
                        else
                        {
                            Console.WriteLine("GCM Notification Failed for some unknown reason");
                        }

                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("Apple Notification Sent!");
                };

                // Start the broker
                apnsBroker.Start();

                // var modelToJson = JsonConvert.SerializeObject(technicianNotification);

                string title = "Tollab";
                string payloadobject = "{\"aps\" :"
                + " {\"alert\" : {\"title\" : \"" + title + "\",\"body\" :\"" + message + "\"},"
                + "\"badge\" : 1" + ", \"sound\":\"default\"}," + "\"NotificationToId\" : " + NotificationToId + "" + "," + "\"ContentId\" : " + ContentId + ""+ "," + "\"Minute\" : " + Minute + "" + "," + "\"VideoQuestionId\" : " + VideoQuestionId + "" + "," + "\"ReferenceId\" : " + ReferenceId + " }";
                var Payloadd = JObject.Parse(payloadobject);
                // Queue a notification to send
                apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = token,
                    Payload = Payloadd
                });

                // Stop the broker, wait for it to finish   
                // This isn't done after every message, but after you're
                // done with the broker
                apnsBroker.Stop();

            }
            catch (Exception)
            {

            }

        }

        public static void pushToAndroidDevice(String token,string message, long? NotificationToId, long? ReferenceId, long? ContentId,long? VideoQuestionId, float? Minute=0)
        {
            string AppId = ConfigurationManager.AppSettings["Push.Android.AppId"];
            string ServerToken = ConfigurationManager.AppSettings["Push.Android.ServerToken"];

            // Configuration
            var config = new GcmConfiguration(AppId, ServerToken, null);
            config.OverrideUrl("https://fcm.googleapis.com/fcm/send");

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();

          //  var modelToJson = JsonConvert.SerializeObject(technicianNotification);

            // Queue a notification to send
            gcmBroker.QueueNotification(new GcmNotification
            {
                RegistrationIds = new List<string> {
                                        token
                                    },

                Data = JObject.Parse("{ \"message\" : \"" + message + "\",\"ReferenceId\" : \"" + ReferenceId + "\",\"ContentId\" : \"" + ContentId + "\",\"Minute\" : \"" + Minute + "\",\"VideoQuestionId\" : \"" + VideoQuestionId + "\",\"NotificationToId\" : \"" + NotificationToId + "\"}")
            });


            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();

        }
        public static void pushBulkToAndroidDevice(List<String> tokens, string message, long? NotificationToId, long? ReferenceId, long? ContentId,long? VideoQuestionId, float? Minute = 0)
        {
            string AppId = ConfigurationManager.AppSettings["Push.Android.AppId"];
            string ServerToken = ConfigurationManager.AppSettings["Push.Android.ServerToken"];

            // Configuration
            var config = new GcmConfiguration(AppId, ServerToken, null);
            config.OverrideUrl("https://fcm.googleapis.com/fcm/send");

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();

            //  var modelToJson = JsonConvert.SerializeObject(technicianNotification);

            // Queue a notification to send
            gcmBroker.QueueNotification(new GcmNotification
            {
                RegistrationIds = tokens,

                Data = JObject.Parse("{ \"message\" : \"" + message + "\",\"ReferenceId\" : \"" + ReferenceId + "\",\"ContentId\" : \"" + ContentId + "\",\"VideoQuestionId\" : \"" + VideoQuestionId + "\",\"Minute\" : \"" + Minute + "\",\"NotificationToId\" : \"" + NotificationToId + "\"}")

            });


            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();

        }
    }
}
