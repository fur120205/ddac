using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Propmaster.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Propmaster.Controllers
{
    public class ServiceBusController : Controller
    {
        IConfiguration configuration;
        static IQueueClient queueClient;
        static List<string> items;
        static List<Booking> bookings;
        bool working = false;
        public ServiceBusController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        //Received Message from the Service Bus - cal get data function
        public async Task<IActionResult> Index()
        {
            items = new List<string>();
            bookings = new List<Booking>();
            if (!working)
            {
                await Task.Factory.StartNew(() =>
                {
                    queueClient = new QueueClient(configuration["QueueConnectionString"], configuration["QueueName"], ReceiveMode.PeekLock);
                    var options = new MessageHandlerOptions(ExceptionMethod)
                    {
                        MaxConcurrentCalls = 1,
                        AutoComplete = false
                    };
                    queueClient.RegisterMessageHandler(ExecuteMessageProcessing, options);
                });
                working = true;
            }
            //queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);

            return RedirectToAction("ProcessMsgResult");
        }

        //public async Task<IActionResult> ProcessMsg()
        //{
        //    //queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
        //    items = new List<string>();
        //    await Task.Factory.StartNew(() =>
        //    {
        //        queueClient = new QueueClient(configuration["QueueConnectionString"], configuration["QueueName"], ReceiveMode.PeekLock);
        //        var options = new MessageHandlerOptions(ExceptionMethod)
        //        {
        //            MaxConcurrentCalls = 1,
        //            AutoComplete = false
        //        };
        //        queueClient.RegisterMessageHandler(ExecuteMessageProcessing, options);
        //    });

        //    return RedirectToAction("ProcessMsgResult");
        //}

        private static async Task ExecuteMessageProcessing(Message message, CancellationToken arg2)
        {
            //var result = JsonConvert.DeserializeObject<Ostring>(Encoding.UTF8.GetString(message.Body));
            // Console.WriteLine($"Order Id is {result.OrderId}, Order name is {result.OrderName} and quantity is {result.OrderQuantity}");
            Debug.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            items.Add("Received message: SequenceNumber:" + message.SystemProperties.SequenceNumber + " Body:" + Encoding.UTF8.GetString(message.Body));
            string json = Encoding.ASCII.GetString(message.Body);
            //string json = BitConverter.ToString(message.Body);
            Booking booking = JsonConvert.DeserializeObject<Booking>(json);
            bookings.Add(booking);
        }

        private static async Task ExceptionMethod(ExceptionReceivedEventArgs arg)
        {
            await Task.Run(() =>
            Debug.WriteLine($"Error occured. Error is {arg.Exception.Message}")
           );
        }

        public IActionResult ProcessMsgResult()
        {
            return View(bookings);
        }
    }
}
