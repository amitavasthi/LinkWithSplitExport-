var UserValidation = {
    ActivateUser: function () {
        document.getElementById("lblUserValidationError").style.visibility = "hidden";

        var inputs = document.getElementsByName("ctlUserValidationValue");

        var parameters = "";

        for (var i = 0; i < inputs.length; i++) {
            parameters += inputs.item(i).getAttribute("Field") + "=" + inputs.item(i).value + "&";
        }

        parameters = parameters.substr(0, parameters.length - 1);

        _AjaxRequest("/Handlers/UserValidationService.ashx", "Validate", parameters, function (response) {
            if (response == "false") {
                document.getElementById("lblUserValidationError").style.visibility = "visible";
                return;
            }
            else if (response == "password") {
                UserValidation.RenderPasswordChange();
            }
            else {
                window.location = window.location;
            }
        });
    },
    RenderPasswordChange: function () {
        var pnlUserValidation = document.getElementById("pnlUserValidation");

        pnlUserValidation.innerHTML = "";

        var html = "<h1 class=\"Color1\">" + LoadLanguageText("UserValidationPasswordTitle") + "</h1>";

        html += "<table>";

        html += "<tr><td>" + LoadLanguageText("NewPassword") + "</td>" +
            "<td><input id=\"txtUserValidationPassword\" type=\"password\" onkeyup=\"UserValidation.ValidatePassword();\" /></td>" +
            "<td><img id=\"imgUserValidationPassword\" style=\"height:20px;\" src=\"/Images/Icons/PasswordInvalid.png\" /></td></tr>";
        html += "<tr><td>" + LoadLanguageText("ConfirmPassword") + "</td>" +
            "<td><input id=\"txtUserValidationPasswordRepeat\" type=\"password\" onkeyup=\"UserValidation.ValidatePassword();\" /></td>" +
            "<td><img id=\"imgUserValidationPasswordRepeat\" style=\"height:20px;\" src=\"\" /></td></tr>";

        html += "<tr><td colspan=\"3\" align=\"right\"><input type=\"button\" " +
            "onclick=\"UserValidation.ActivatePasswordChange();\" value=\"" +
            LoadLanguageText("UserValidationButtonActivate") + "\"></td></tr>";

        html += "</table>";

        pnlUserValidation.innerHTML = html;
    },
    ActivatePasswordChange: function () {
        if (UserValidation.ValidatePassword() == false)
            return;

        _AjaxRequest("/Handlers/UserValidationService.ashx", "ChangePassword", "Password=" + document.getElementById("txtUserValidationPassword").value, function (response) {
            if (response == "false") {
                UserValidation.ValidatePassword();
                return;
            }
            else {
                window.location = window.location;
            }
        });
    },
    ValidatePassword() {
        var txtUserValidationPassword = document.getElementById("txtUserValidationPassword");
        var txtUserValidationPasswordRepeat = document.getElementById("txtUserValidationPasswordRepeat");

        var imgUserValidationPassword = document.getElementById("imgUserValidationPassword");
        var imgUserValidationPasswordRepeat = document.getElementById("imgUserValidationPasswordRepeat");

        imgUserValidationPasswordRepeat.src = "";
        
        if (txtUserValidationPassword.value.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}/i) == undefined) {
            imgUserValidationPassword.src = "/Images/Icons/PasswordInvalid.png";
            return false;
        }
        else {
            imgUserValidationPassword.src = "/Images/Icons/PasswordValid.png";
        }

        if (txtUserValidationPassword.value != txtUserValidationPasswordRepeat.value) {
            imgUserValidationPasswordRepeat.src = "/Images/Icons/PasswordInvalid.png";
            return false;
        }

        imgUserValidationPasswordRepeat.src = "";

        imgUserValidationPassword.src = "/Images/Icons/PasswordValid.png";
        imgUserValidationPasswordRepeat.src = "/Images/Icons/PasswordValid.png";
        return true;
    }
}