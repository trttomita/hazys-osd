#UI localization for config tool.

# Introduction #

Many friends are interested in translate the config tool into their own language, and we really appreciate their willing to help. In this guide, I'll introduce two different ways for helping us translate the config tools.


---

# Simplest Way - Text Translation Only #

Since there are actually not many texts in the config tool, I write all of them in a plain text file ([here](http://hazys-osd.googlecode.com/svn/wiki/language.txt)). You can simply translate them into your language, and send the translation to me (hazyhxj@gmail.com). Please keep the English text, and write the translation in a new line under each English text so that I can match them up.

The lines like

```
/************* OSD Function Names *************/
/************* Prompt Messages ****************/
/************* Form UI ************************/
```

are just for separating the sections in the text, you do not have to translate them :)


---

# Complex Way - Using Programming Environment #

Maybe you want to know how it works, or you want to adjust the looking in your language (e.g. A label isn't long enough to show the text), you can use the Programming Environment to modify the code. That's actually how I integrate the language translation into the project.

## What do you need ##
  * Visual C# Express 2010. This one is completely free, you can download from [here](http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express). You can also use the commercial Visual Studio 2010 (are you that rich?). There are trial visions [here](http://www.microsoft.com/visualstudio/en-us/try).
  * A svn tool and a Google Account. Refer to [download and upload code files](svn.md) for more details.

## Translate the main form ##
The Visual Studio provides a simple UI designer that suppose localization of forms. In this step we'll use this designer to edit the main form.

1. Open the project using Visual C# or Visual Studio. Sometimes the Visual Studio may crash at startup. I haven't figure out the reason, but you can close Visual Studio and open it again, then it will be ok.

2. Open the designer for "OSDConfigForm" by double click the item.

3. Select the form by click on the title bar of the form. Do not select the components in the form.

4. Navigate to the "Language" property in the Property Window, change the language into your own language.

5. Edit the components in the Form. Click each components, navigate to the "Text" property, and write down the caption in your language. Sometime you may also want to edit the "ToolTipText" property.

You will notice a new file named "OSDConfigForm.LANG.resx" are created automatically, where "LANG" stands for code for your language, e.g. "es" for Spanish, "pl" for Polish, etc. Remember this code, you will need them in the following step.

## Translate the function names and messages ##
For the consideration of further upgrade, the function names are loaded dynamically from a resource file. The prompt messages are also loaded from a resource file. Unlike the localization of Form, the Visual Studio does not provide a simple UI to create localized resource files, and you have to add them manually.

1. Make copies of "OSDItemNames.resx" and "Messages.resx", and name them as "OSDItemNames.LANG.resx" and "Messages.LANG.resx". Here "LANG" stands for the code for your language, e.g. "en" for English, "es" for Spanish. Refer to Step 2 to check the code if you are not sure.

2. Add the copied resource file into the project.

3. Open the resource file by double click the name, and translate the texts into your language.

**Note** Actually you may also edit the resources of the main form using this way. Copy the "OSDConfigForm.resx", rename it accordingly and then edit the text in the file. I suggest not to do this way because this file also contains many other meta data that shouldn't be edited manually, and thus it may be difficult to find the right items for translation.