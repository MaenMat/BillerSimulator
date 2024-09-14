using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BillerSimulator.Customers
{
    public class Customer : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? CustomerNumber { get; set; }

        [NotNull]
        public virtual string FullName { get; set; }

        [NotNull]
        public virtual string PhoneNumber { get; set; }

        [CanBeNull]
        public virtual string? Email { get; set; }

        [CanBeNull]
        public virtual string? Address { get; set; }

        public Customer()
        {

        }

        public Customer(Guid id, string customerNumber, string fullName, string phoneNumber, string email, string address)
        {

            Id = id;
            Check.NotNull(fullName, nameof(fullName));
            Check.NotNull(phoneNumber, nameof(phoneNumber));
            CustomerNumber = customerNumber;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }

    }
}