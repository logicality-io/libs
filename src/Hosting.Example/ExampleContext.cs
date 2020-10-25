namespace Logicality.Extensions.Hosting.Example
{
    public class ExampleContext
    {
        private readonly HostedServiceBag _hostedServices = new HostedServiceBag();

        public Simple1HostedService Simple1
        {
            get => _hostedServices.Get<Simple1HostedService>(nameof(Simple1));
            set => _hostedServices.Add(nameof(Simple1), value);
        }

        public Simple2HostedService Simple2
        {
            get => _hostedServices.Get<Simple2HostedService>(nameof(Simple2));
            set => _hostedServices.Add(nameof(Simple2), value);
        }

        public Simple3HostedService Simple3
        {
            get => _hostedServices.Get<Simple3HostedService>(nameof(Simple3));
            set => _hostedServices.Add(nameof(Simple3), value);
        }

        public Simple4HostedService Simple4
        {
            get => _hostedServices.Get<Simple4HostedService>(nameof(Simple4));
            set => _hostedServices.Add(nameof(Simple4), value);
        }

        public Simple5HostedService Simple5
        {
            get => _hostedServices.Get<Simple5HostedService>(nameof(Simple5));
            set => _hostedServices.Add(nameof(Simple5), value);
        }
        public SeqHostedService Seq
        {
            get => _hostedServices.Get<SeqHostedService>(nameof(Seq));
            set => _hostedServices.Add(nameof(Seq), value);
        }
    }
}
