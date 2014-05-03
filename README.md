Background
=================

Akshar was created in late 2009-early 2010 and was featured in semifinal of [Microsoft Imagine Cup](http://www.imaginecup.com) world-wide Next Generation Web Award (2010) competition. [Watch Akshar multilingual capabilities on Youtube](http://www.youtube.com/embed/UitLcDIWOU8?rel=0)

Introduction
=================

Akshar (a term for character/letter in Hindi) is an accessible, embeddable, highly extensible, multilingual online word processor for all. It Features:

-   Ability to type in any language out of the box, without installing any add-on or using any specific language services provided by the host operating system.

    For a quick demonstration, [watch This video](http://www.youtube.com/embed/UitLcDIWOU8?rel=0). Aforementioned video shows a sentence (briefing Akshar's features) being written in 8 different languages. It accompanies the audio description in which the same sentence is recited along, however in English only (obviously the purpose here is to demonstrate multilingual tcapability of Akshar and not mine :)).

-   The ability to easily create and/or customize a key/character mappings called Virtual Keyboard Layout (VKL) for a language, while also maintaining a single default VKL for each language. It also allows users to share their personal VKLs.

    The multilingual feature of Akshar depends on this VKL technology. I have tried to develop an intuitive interface to enable users to quickly and easily create/edit VKLs. The interface also makes it very easy for users to customize existing VKLs (even the ones created by others).

    [Learn more](#user-content-vkl-faq) about VKL.

-   The ability to embed this VKL technology into third-party sites simply with the inclusion of a publically available JavaScript file, enabling their users to easily express themselves in regional languages.

    This embeddable script, named Virtual Keyboard Manager (VKM), fetches requested VKLs from a web service realizing their reuse, sharing and centralized management. VKL is internally represented as Extensible Markup Language (XML) which makes it ideal to be shared and used across the web with diverse development technologies. This also means that it would be possible for people to create custom VKM for specific needs.

    [Learn more](#user-content-embed)

-   The ability to expose VKL input to accessibility clients (such as screen readers) with specific markup attributes.

    [Learn more](#user-content-access-vkl) how screen readers (such as Jaws) may detect VKL enabled text controls, and capture the text generated when a key is pressed.

-   Akshar takes accessibility of online rich editors a step further by providing keyboard alternatives to formatting, navigation and editing commands.

    [Learn more](#user-content-accessibility) about Akshar accessibility features and how can they enrich online word processing experience for people with disability.

<a name="vkl-faq"></a>Creating Virtual Keyboard Layout (VKL): Frequently Asked Questions
==================================================================

What is VKL?  
A VKL is an XML file that contains key/character mappings. Every key on a keyboard has a unique scan code (a number) which is provided to the operating system when a key is pressed. Although these scan codes may differ from one keyboard vender to another, the operating system hides this peculiarity by substituting these scan codes with standard key codes. So, while creating a VKL, you specify what character is generated when a particular key is pressed.

What is the difference between Key based and Character based VKL?  
In a Key based VKL, you specify what character is generated for a particular key code; on the other hand, in a Character based VKL, you specify a character against character. This means that in the latter case, when a particular character is generated by the system (regardless of the key pressed and key code generated), only then the associated character will be printed by VKM. Surely, Character based VKL is subject to the keyboard layout loaded within the system itself and is provided to overcome the problem of varying key codes within operating systems from different vendors. (Though its applicability is still a subject of scrutiny.)

What is a sequence?  
A sequence allows users to create keystroke sequences. The first keystroke (also known as parent map) does not cause any character to be printed. Rather, it begins a sort of sub VKL such that when you press the next keystroke, it will be mapped against the sub mappings you've defined for this parent map.

What is a rule?  
A rule defines �When this key is pressed, output this character if and only if, the cursor is adjacent to a particular text in a particular direction replacing that text.� The direction can be either right or left:

-   Right captures the current character (where cursor is positioned) and characters that follow it for rule matching,
-   Left captures characters preceding the current character (where cursor is positioned) excluding it for rule matching.

 There are two types of Rules:

-   Text � used to look for a literal text
-   Range � used to look for a range of characters (using simple [] regular expression patterns).

 A rule cannot match more than 3 characters and cannot have spaces to be matched.

<a name="embed"></a>Embed Multilingual Editing Support in Your Website
==================================================

One of the primary efforts behind Akshar is to create an infrastructure that provides for unconstrained and independent mechanism for writing in any language, without any specific support from technology not built into the web. The goal is intrinsically related to free the web from such support as well, and this objective calls for Akshar�s VKL technology to be embeddable in any site.

Follow These Four Simple Steps and have Your Site Ready to be Truly Multilingual!
---------------------------------------------------------------------------------

1.  First find out a Virtual Keyboard Layout (VKL) for the language you like to enable editing into. From the Language menu, choose add ..., and from the dialog box that appears, choose the language and note down the name of VKL you desire. Also remember the type of VKL you choose (Key based is usually preferred).

    Couldn�t find a VKL for your language? No problem! You can create your own. It�s very easy!

2.  Put the following reference to the Virtual Keyboard Manager (VKM) in the appropriate page:

    \<script src="http://varunk.net/vkm-0.4.8.js" type="text/javascript"\>\</script\>

3.  Create a JavaScript script tag and write the following function:

        <script language="javascript">
            function initVKM() {
                    registerTextCtrl("textControlId", "VKLName", "KEY");
            }
        </script>

4.  Substitute parameters according to your requirements and selection.

That�s it! You are done!

<a name="accessibility"></a>Accessibility of Akshar
=======================

Accessibility of Akshar is not a coincident; right from the outset, it has been built with a vision of an online word processor that must be accessible. Here are some of the accessibility initiatives I have taken for Akshar:

Quick Command
-------------

Quick Command (Hereafter pronounced as QuiC) is a method for users to issue various types of commands with keyboard while working in the edit area. Although it may sound similar to keyboard shortcuts feature found in most desktop applications, but it is quite different and more powerful than shortcuts feature.

QuiC works as follows: while focus is in the Akshar�s edit area, user presses the QuiC key (shift+3). Thereafter, every key input is captured in a command buffer rather than being rendered on the screen. When user presses the space key, QuiC mode terminates and if a valid command is found in the QuiC buffer, it is executed. If user presses QuiC key twice, the key is passed and the buffering does not start. User can, any time, terminate QuiC mode by pressing the QuiC key. While in QuiC mode, the backspace key deletes the last character entered in the buffer.

Following is a categorized list of QuiC commands that are available in this first version of Akshar:

Note: QuiC commands are not case-sensitive for convenience.

### Formatting Commands

I  
Toggles italic on the selected text.

B  
Toggles bold on the selected text.

U  
Toggles underline on the selected text.

UL  
Inserts unordered list

OL  
Inserts ordered list

SUP  
Toggles superscript on selected text

SUB  
Toggles subscript on selected text

JL  
Left-justifies the format block in which the current selection is located.

JC  
Centers the format block in which the current selection is located.

JR  
Right-justifies the format block in which the current selection is located.

FC (followed by standard colors� names or a hexadecimal color value)  
Sets the foreground (text) color of the current selection.

Examples: (both apply red color)

1.  FCred
2.  FCff0000

BC (followed by standard colors� names or a hexadecimal color value)  
Sets the background (text) color of the current selection.

Examples: (both apply red color)

1.  BCred
2.  BCff0000

CLS  
Removes the formatting tags from the current selection.

FS (followed by a value between 1 and 7, inclusive)  
Sets the font size for the current selection.

Note: The underlying infrastructures of some browsers and/or specific versions of a browser may interpret the actual command (converted by QuiC) differently and thus a specific command may not behave the way it is mentioned here.

### Navigation Commands

1/or any other number  
Switches to the document as indicated by its position in the Document menu (the first listed document in the menu has number 1).

M  
Sets focus to the menu bar (File menu)

MF  
Opens file menu and sets focus on first menu item.

MV  
Opens View menu and sets focus on first menu item.

ML  
Opens Language menu and sets focus on first menu item.

MD  
Opens Document menu and sets focus on first menu item.

MT  
Opens Tools menu and sets focus on first menu item.

MH  
Opens Help menu and sets focus on first menu item.

### File Related Commands

N  
Creates a new, empty document.

O  
Opens Open dialog and sets focus on it.

S  
Issues Save command

SA  
Opens Save as dialog and sets focus on it.

C  
Issues Close command

Other Akshar�s Accessibility Features
-------------------------------------

Although QuiC provides an enhanced method for issuing various types of commands, all attempts have been made to make menus and dialog boxes fully accessible by themselves.

-   users can easily navigate menu system and dialog boxes using the tab key. When a sub menu/dialog box is opened, it automatically gets the focus. it can subsequently be terminated by pressing the escape key.
-   The Accessible Rich Internet Applications Suite (WAI-ARIA) guidelines have been implemented to provide:

    -   A desktop-like menu interface � if you press enter/space bar to activate a top-menu (e.g. File), you enter into this mode. You can use Up or Down arrow keys to move to a previous or a next menu item, respectively within the same popup menu. Similarly, using Left or Right arrow keys, you can move across popup menus. Pressing escape once collapses the current popup menu and focus shifts onto the associated top-level menu. If you press escape once again, the focus will turn to the document edit area.

        Note: if your screen reader intercepts the escape key to turn off the forms mode (Jaws does this), you must press escape twice and thrice to achieve the above functionalities respectively.

    -   division of the application using landmarks.

    Note: for above WAI-ARIA features to work for you, your screen reader must support them in the first place.

<a name="access-vkl"></a>Virtual Keyboard Layout (VKL) Accessibility
-------------------------------------------

The VKL technology works in JavaScript by intercepting keyboard input. The accessibility clients (such as screen readers) intercept keyboard input at the much lower level (by using system hooks). Hence, in usual scenario, such applications won�t be able to capture text generated by VKL input unless they attempt to do the following:

1.  A VKL bound control has an attribute vklInput defined which can be easily detected by screen readers and other accessibility clients.
2.  Once an accessibility client has detected that a control is VKL bound, it should refrain from speaking the key input in a way as it usually does. Rather, it should wait, and let the key be passed to the browser application so that it can be processed by VKM.
3.  Once a key input is processed, if it results in some text, the VKL bound control will have a keyEnter attribute (a flag) defined and the actual generated text can be extracted from the attribute named vklText.

I had created an experimental Jaws script file to demonstrate how easy it is to understand and process VKL by a screen reader (in this case no particular support was required from the screen reader vendor!)


License
=================

No part of this software can be used for commercial purposes without a written consent of the author (Varun Khosla)

Copyright 2010-2011 Varun Khosla. All rights reserved.