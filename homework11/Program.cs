using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace homework11
{
    class Program
    {
        private static List<Client> listClients = new()
        {
            new Client {Id = 1, Balance = 100m, Name = "Vasya"},
            new Client {Id = 2, Balance = 0m, Name = "TojeVasya"}
        };

        private static List<Client> _shadowClients = new(listClients.Count);

        private static object locker = new();

        static void Main(string[] args)
        {
            _shadowClients.Clear();
            listClients.ForEach(item =>
                _shadowClients.Add(new Client {Id = item.Id, Balance = item.Balance, Name = item.Name}));

            var timerCallback = new TimerCallback(CheckDiff);
            var _ = new Timer(timerCallback, null, 0, 500);

            while (true)
            {
                // esli vsyo ostavit' tak, to teksti kotorie budut vivodit'sya budut peresekat'sya drug s drugom
                // po yetomu postavlyu zdes' Thread.Sleep
                Thread.Sleep(100);
                fmt.Println("┌──────────────────────────────────┐", ConsoleColor.Green);
                fmt.Println("│  choose action to proceed:       │", ConsoleColor.Green);
                fmt.Println("│    1 => add new client           │", ConsoleColor.Green);
                fmt.Println("│    2 => delete existing client   │", ConsoleColor.Green);
                fmt.Println("│    3 => select all               │", ConsoleColor.Green);
                fmt.Println("│    4 => update existing client   │", ConsoleColor.Green);
                fmt.Println("│    q => exit                     │", ConsoleColor.Green);
                fmt.Println("└──────────────────────────────────┘", ConsoleColor.Green);

                switch (fmt.Scan().Trim())
                {
                    case "1":
                    {
                        fmt.Print("client name: ");
                        var client = new Client {Name = fmt.Scan()};
                        var insertThread = new Thread(Insert) {Name = "InsertThread"};
                        insertThread.Start(client);
                        break;
                    }
                    case "2":
                    {
                        fmt.Print("client id: ");
                        var client = new Client {Id = Convert.ToInt32(fmt.Scan())};
                        var deleteThread = new Thread(Delete) {Name = "DeleteThread"};
                        deleteThread.Start(client);
                        break;
                    }
                    case "3":
                    {
                        var selectThread = new Thread(Select) {Name = "SelectThread"};
                        selectThread.Start();
                        break;
                    }
                    case "4":
                    {
                        fmt.Print("client id: ");
                        var id = Convert.ToInt32(fmt.Scan());
                        fmt.Print("balance: ");
                        var balance = Convert.ToDecimal(fmt.Scan());
                        fmt.Print("(not required, leave blank) name: ");
                        var client = new Client {Id = id, Balance = balance, Name = fmt.Scan()};
                        var updateThread = new Thread(Update) {Name = "UpdateThread"};
                        updateThread.Start(client);
                        break;
                    }
                    case "q":
                    {
                        fmt.Println("bye");
                        return;
                    }
                    default:
                    {
                        fmt.Println("invalid action", ConsoleColor.Red);
                        break;
                    }
                }
            }
        }

        private static void Insert(object client)
        {
            Monitor.Enter(locker);

            if (client is Client tmpClient)
            {
                tmpClient.Id = listClients.Count == 0 ? 1 : listClients.Last().Id + 1;

                listClients.Add(tmpClient);
            }

            _shadowClients.Clear();
            listClients.ForEach(item =>
                _shadowClients.Add(new Client {Id = item.Id, Balance = item.Balance, Name = item.Name}));

            Monitor.Exit(locker);
        }

        private static void Update(object client)
        {
            Monitor.Enter(locker);
            _shadowClients.Clear();
            listClients.ForEach(item =>
                _shadowClients.Add(new Client {Id = item.Id, Balance = item.Balance, Name = item.Name}));

            if (client is Client tmpClient)
            {
                foreach (var t in listClients.Where(t => t.Id == tmpClient.Id))
                {
                    t.Balance = tmpClient.Balance;
                    if (!tmpClient.Name.Trim().Equals(""))
                    {
                        t.Name = tmpClient.Name;
                    }

                    break;
                }
            }

            Monitor.Exit(locker);
        }

        private static void Delete(object client)
        {
            Monitor.Enter(locker);

            if (client is Client tmpClient)
            {
                foreach (var t in listClients.Where(t => t.Id == tmpClient.Id))
                {
                    listClients.Remove(t);
                    break;
                }
            }

            _shadowClients.Clear();
            listClients.ForEach(item =>
                _shadowClients.Add(new Client {Id = item.Id, Balance = item.Balance, Name = item.Name}));

            Monitor.Exit(locker);
        }

        private static void Select()
        {
            Monitor.Enter(locker);

            foreach (var client in listClients)
            {
                fmt.Println($"id:{client.Id}, name: {client.Name}, balance:{client.Balance}");
            }

            Monitor.Exit(locker);
        }

        private static void CheckDiff(object _)
        {
            for (var i = 0; i < listClients.Count; i++)
            {
                if (listClients[i].Id != _shadowClients[i].Id) continue;

                if (listClients[i].Balance < _shadowClients[i].Balance)
                {
                    fmt.Println(
                        $"id: {listClients[i].Id}, before: {_shadowClients[i].Balance}, after: {listClients[i].Balance}, difference: -{Math.Abs(listClients[i].Balance - _shadowClients[i].Balance)}",
                        ConsoleColor.Red);
                }
                else if (listClients[i].Balance > _shadowClients[i].Balance)
                {
                    fmt.Println(
                        $"id: {listClients[i].Id}, before: {_shadowClients[i].Balance}, after: {listClients[i].Balance}, difference: +{Math.Abs(_shadowClients[i].Balance - listClients[i].Balance)}",
                        ConsoleColor.Green);
                }
            }

            _shadowClients.Clear();
            listClients.ForEach(item =>
                _shadowClients.Add(new Client {Id = item.Id, Balance = item.Balance, Name = item.Name}));
        }
    }

    public class Client
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
    }
}