namespace dotnet_pizza_protocol {
    public interface IPizzaSerde<S> {
        byte[] Serialize();
        abstract static S? Deserialize(byte[] bytes, int length);
    }
}