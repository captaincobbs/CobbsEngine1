using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cobbs_Engine
{
    public static partial class Diagnostics
    {
        static string htmlTemplate = @"<script>
    // Toggle stack
    function toggleStackTrace(link) {
        var div = link.closest("".log"");
        if (div) {
            var pre = div.querySelector(""pre"");
            if (pre) {
                if (pre.style.display === ""none"" || pre.style.display === """") {
                    pre.style.display = ""block"";
                } else {
                    pre.style.display = ""none"";
                }
            }
        }
    }

    // Filter by category
    function toggle_logs_by_class(className, button){
        var elements = document.querySelectorAll(""div."" + className);

        console.log(className)
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            element.classList.toggle('hidden');
        }

        button.classList.toggle('disabled');
    }
</script>
<style>
    /* Body */
    body {
        font-family:""calibri"";
        margin: 0;
        padding: 0;
        background: rgb(210, 210, 210);
    }

    body.message div.message,
    body.warning div.warning,
    body.error div.error,
    body.exception div.exception,
    body.assert div.assert
    body.debug div.debug {
        display: block;
    }

    /* Category Coloring */
    div.message, input.message:not(.disabled) {
        background-color:#eeeeee;
    }

    div.warning, input.warning:not(.disabled) {
        background-color:#FF8F00;
        color:#FFF8E1;
    }

    div.error, input.error:not(.disabled) {
        background-color:#FF2A2A;
        color:#FFEBEE;
    }

    div.exception, input.exception:not(.disabled) {
        background-color:#C2185B;
        color:#FCE4EC;
    }

    div.assert, input.assert:not(.disabled) {
        background-color:#4527A0;
        color:#EDE7F6;
    }

    div.debug, input.debug:not(.disabled) {
        background-color:#212aad;
        color:#EDE7F6;
    }

    /* Header */
    .button {
        border:none;
        cursor: hand;
        padding:10px;
        width:100%;
        font-weight:bold;
    }

    .button-bar {
        display: flex;
        justify-content: center;
        width: 100%;
        margin: 0;
        padding: 0;
    }

    Header {
        position: sticky;
        width: 100%;
        box-shadow: 2px 2px 2px rgba(0, 0, 0, 0.4), 0 5px 5px rgba(0, 0, 0, 0.2);

        background-color: white;
        display:inline-block;
        margin: 0px;
        top:0;
        z-index: 999;
    }

    h1 {
        margin-block-end: -0.2em;
        margin-block-start: 0;
        padding-left:2px;
        padding-right:2px;
    }

    h2 {
        margin-block-end: 0;
        margin-block-start: 0;
        padding-left:2px;
        padding-right:2px;
    }

    .button.disabled {
        background-color: gray;
        color:white;
    }

    /* Content */
    .log {
        margin-top: 0;
        padding-top: 7x;
        padding-bottom: 7px;
    }

    pre {
        display:none;
        background: rgba(120, 120, 120, 0.5);
        padding-bottom:0;
        padding-top: 0;
        margin: 10px 0 0 0;
        padding-left: 20px;
        font-family: 'Courier New', Courier, monospace;
        border-radius: 12px;
        overflow-x: auto;
    }

    .flags {
        background-color:transparent;
        float: left;
        width:6em;
    }

    a {
        background-color:#ffffff;
        color:rgb(120, 120, 120);
        cursor: hand;
        font-size:small;
        font-weight:normal;
        margin-right: 16px;
        padding:3px;
        text-decoration: none;
        width:10em;
        border-radius: 5px;
    }

    content > div {

        font-family:""calibri"";
        margin:2px;
        padding:4px;
    }

    .time {
        background-color:transparent;
        color:#757575;
        float: left;
        width:8em;
    }

    .hidden {
        display: none;
    }
</style>
<main>
    <header>
        <h1>{GameName}</h1>
        <h2>{InitialTime}</h2>
        <span style=""padding-left: 2px; padding-bottom: 8px;display:block;"">Click on the buttons to toggle visibility. Click on STACK buttons to toggle visibility of stack traces.</span>

        <div class=""button-bar"">
            <input type=""button"" value=""Message"" class=""log button"" onclick=""toggle_logs_by_class('message', this)"" />
            <input type=""button"" value=""Warning"" class=""warning button"" onclick=""toggle_logs_by_class('warning', this)"" />
            <input type=""button"" value=""Error"" class=""error button"" onclick=""toggle_logs_by_class('error', this)"" />
            <input type=""button"" value=""Exception"" class=""exception button"" onclick=""toggle_logs_by_class('exception', this)"" />
            <input type=""button"" value=""Assert"" class=""assert button"" onclick=""toggle_logs_by_class('assert', this)"" />
            <input type=""button"" value=""Debug"" class=""debug button"" onclick=""toggle_logs_by_class('debug', this)"" />
        </div>
    </header>
    <div style=""height:10px""></div>
    <content></content>
</main>";
    }
}
