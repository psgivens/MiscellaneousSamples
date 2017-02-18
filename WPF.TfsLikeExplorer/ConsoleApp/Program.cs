using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ProducerConsumerEngine;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {

            DoIt(async () => await Task.Delay(10));
            

            var builder = new ContainerBuilder();
            builder.RegisterSource(new ObserverRegistrationSource());
            builder.RegisterType<FirstObserver>().As<IObserver>();
            builder.RegisterType<SecondObserver>().As<IObserver>();
            builder.RegisterType<ActionableItem>();
            builder.RegisterType<Dependency>();

            IContainer container = builder.Build();
            var item = container.Resolve<ActionableItem>();
        }
        private static void DoIt(Action action) {

        }

        private static void DoIt(Func<Task> action) {

        }


    }

    



    public interface IObserver {
        void OnAction();
    }

    public class FirstObserver : IObserver {
        void IObserver.OnAction() {
            throw new NotImplementedException();
        }
    }
    public class SecondObserver : IObserver {
        public SecondObserver(Dependency dependency) { 
        }
        void IObserver.OnAction() {
            throw new NotImplementedException();
        }
    }

    public class Dependency { }

    public class ActionableItem {
        public ActionableItem(Observer<IObserver> observer) {
            observer.Value.OnAction();
        }

    }
}
