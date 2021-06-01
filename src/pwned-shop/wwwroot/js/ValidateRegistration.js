window.onload = function ()
{

}


function ValidateForm()
{
    if ((ValidateFirstName() == false) || (ValidateLastName() == false) || (ValidateEmail() == false) || (ValidatePassword() == false) || (ValidateDOB() == false))
    {
        return false;
    }

    }
    function ValidateFirstName()
    {
        var x = document.forms["myForm"]["FirstName"].value;
        if (x == "")
        {
            alert("First name must be filled out");
            return false;
        }
        else if (x.length > 30)
        {
            alert("First name exceeded the character limit");
            return false;
        }

    }
    function ValidateLastName()
    {
        var y = document.forms["myForm"]["LastName"].value;
        if (y == "")
        {
            alert("Last name must be filled out");
            return false;
        }
        else if (y.length > 30)
        {
            alert("Last name exceeded the character limit");
            return false;
        }
    }
    function ValidateEmail() {
        var z = document.forms["myForm"]["Email"].value;
        if (z == "") {
            alert("Email must be filled out");
            return false;
        }
        else if (z.length > 25) {
            alert("Email exceeded the character limit");
            return false;
        }
    }
    function ValidatePassword()
    {
        var b = document.forms["myForm"]["Password"].value;
        if ((b.length < 8)|| (b.indexOf(" ") != -1))
        {
            alert("Password must contain at least 8 characters (only alphanumeric or special characters like %!?$ are allowed)");
            return false;
        }
    }
    function ValidateDOB()
    {
        var c = document.forms["myForm"]["DOB"].value;
        if (c == "")
        {
            alert("Date of Birth must be filled out");
            return false;
        }

    }

