﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using BackupStatus.Models;
using System.Data;
using System.Data.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackupStatus.Client
{
    class ClientMain
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            string srvName = Dns.GetHostName();
            HttpResponseMessage response;
            SQLiteConnection sqliteConnection;
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            string jsonMsg;
            DateTime lastMsgDate;

            StatusCode numStatus;
            Host host;

            // Web service connection
            client.BaseAddress = new Uri (args[0]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = await client.GetAsync("api/UpdateStatus/" + srvName);
            if (response.IsSuccessStatusCode)
            {
                host = await response.Content.ReadAsAsync<Host>();
            }
            else
            {
                throw new HttpRequestException("Erro ao acessar " + client.BaseAddress + ": " + response.StatusCode);
            }

            // Backup software database connection
            Console.WriteLine("Conectando ao banco de dados " + host.DbLocation + "...");
            sqliteConnection = new SQLiteConnection(@"DataSource=" + host.DbLocation);
            sqliteConnection.Open();

            // Get the last log
            using (var cmd = sqliteConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT LD.Message, " +
                                  "       LD.Timestamp " +
                                  "  FROM LogData LD" +
                                  "       INNER JOIN Operation O ON LD.OperationID = O.ID" +
                                  " WHERE O.Description = 'Backup'" +
                                  " ORDER BY LD.Timestamp DESC " +
                                  " LIMIT 1;";
                da = new SQLiteDataAdapter(cmd.CommandText, sqliteConnection);
                da.Fill(dt);
                jsonMsg = dt.Rows[0]["Message"].ToString();
                lastMsgDate = DateTimeOffset.FromUnixTimeSeconds((long)dt.Rows[0]["Timestamp"]).DateTime.ToLocalTime();
            }
            sqliteConnection.Close();

            // Parse the last log and get the result
            var values = JObject.Parse(jsonMsg);
            switch ((string)values["ParsedResult"])
            {
                case "Success":
                    numStatus = StatusCode.OK;
                    break;
                case "Warning":
                    numStatus = StatusCode.WARNING;
                    break;
                case "Error":
                    numStatus = StatusCode.ERROR;
                    break;
                case "Fatal":
                    numStatus = StatusCode.ERROR;
                    break;
                case "Unknown":
                    numStatus = StatusCode.UNKNOWN;
                    break;
                default:
                    numStatus = StatusCode.UNKNOWN;
                    break;
            }

            // Update the status on the system
            try
            {
                host.ReturnCode = numStatus;
                host.LastStatusUpdate = lastMsgDate;
                response = await client.PutAsJsonAsync("api/UpdateStatus/" + host.Id, host);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Erro ao comunicar com o servidor.");
                Console.WriteLine(ex);
                Environment.Exit(-1);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(-2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(-3);
            }
            Environment.Exit(0);
        }
    }
}
