using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    // BusinessLogicLayer/Exceptions/CustomExceptions.cs
    namespace BusinessLogicLayer.Exceptions
    {
        public class EmployeeNotFoundException : Exception
        {
            public EmployeeNotFoundException(Guid employeeId)
                : base($"Employee with ID {employeeId} not found")
            {
            }

            public EmployeeNotFoundException(string message) : base(message)
            {
            }
        }

        public class RoleNotFoundException : Exception
        {
            public RoleNotFoundException(Guid roleId)
                : base($"Role with ID {roleId} not found")
            {
            }

            public RoleNotFoundException(string message) : base(message)
            {
            }
        }

        public class DuplicateEmailException : Exception
        {
            public DuplicateEmailException(string email)
                : base($"Employee with email {email} already exists")
            {
            }
        }

        public class InvalidRoleAssignmentException : Exception
        {
            public InvalidRoleAssignmentException(string message) : base(message)
            {
            }
        }

        public class UnauthorizedAccessException : Exception
        {
            public UnauthorizedAccessException(string message) : base(message)
            {
            }
        }
    }
}
