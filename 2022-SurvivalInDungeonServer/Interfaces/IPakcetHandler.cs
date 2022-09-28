using Main;

public interface IPakcetHandler
{
    PacketType Type { get; }
    void Handle(Session session, Packet packet);
}