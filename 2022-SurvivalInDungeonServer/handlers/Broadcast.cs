using Main;

public class Broadcast : IPakcetHandler
{
    public PacketType Type => PacketType.Broadcast;

    public void Handle(Session session, Packet packet)
    {
        MainServer.Broadcast(packet, session);
    }
}
