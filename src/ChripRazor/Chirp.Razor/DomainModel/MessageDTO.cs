﻿namespace Chirp.Razor.DomainModel;

public class MessageDTO
{
    public string Text { get; set; }
    public User User { get; set; }
    public Int64 Timestamp { get; set; }

}