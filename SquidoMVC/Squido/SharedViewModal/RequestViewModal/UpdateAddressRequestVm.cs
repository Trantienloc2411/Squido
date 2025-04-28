using System;

namespace SharedViewModal.RequestViewModal;

public class UpdateAddressRequestVm
{
    public string? Id { get; set; }    
    public string? HomeAddress { get; set; }
    public string? WardName { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Phone {get;set;}
}
