# DreamCheekyLED

#### A "Free as in speech" library for easy integration with a ThinkGeek classic
<br />
<br />

## Background


The Dream Cheeky USB LED Message Board v1.0.1 launched at the now-defunct ThinkGeek ([product page](https://web.archive.org/web/20140120004039/http://www.thinkgeek.com/product/1690)) on January 15, 2014 <sup>[[1]](https://web.archive.org/web/20140122054607/http://www.thinkgeek.com/whats-new/)</sup>, with no surviving manufacturer software (they had a forum, but archive.org [barely saw it](https://web.archive.org/web/*/http://dreamlink.info/dream/*)). A May 2012 [Wordpress post](https://dgwilson.wordpress.com/2012/05/13/led-message-board/) by a customer added Mac OS 10.7+ support, a 2009 [SourceForge project](https://sourceforge.net/projects/leddisplayctrl/) offers a neat UI with a really janky web server, and a 2011 [console GitHub project](https://github.com/kost/dcled) by a customer written in C is *potentially* useful, but really in a very limited format. However, that same GitHub project links to [a write-up about it](https://www.jeffrika.com/~malakai/dcled/index.html), which has the [Hardware Developer Manual](https://www.jeffrika.com/~malakai/dcled/USB%20LED%20Message%20Board%20-%20Developer%20Manual%20v1.0.pdf). With this manual, I was able to write this library, which -- with [HidSharp](https://github.com/IntergatedCircuits/HidSharp) for the USB protocol handling, and optionally [SkiaSharp](https://github.com/mono/skiasharp)
for image handling) -- includes ***everything*** you need to get started with coding all sorts of projects for a Dream Cheeky LED Message Board, as minimalistically or as full-fledged as possible.

Plus, as a bonus, I've also included a ConsoleApp that you can send filepaths of images to, and it will display those images on the messageboard as long as the app remains open, whether animated GIF or static PNG/GIF. Check out the [Quick Start Guide](quickstart.md) for the quickest way to get started, or read on for a bit more of an introduction :)
<br />
<br />
## The DreamCheekyLED library

This compiles to a simple DLL using the HidSharp NuGet package and handles ***everything*** you need to work with the Dream Cheeky LED USB Message Board at a rudimentary level. No need to compile C# just to get this working, it's language-agnostic. From setting the brightness level to addressing individual LEDs to even an asyncronous method  that handles constant screen refreshes in the background so you don't have to. The DreamCheekyLED library has all the heavy lifting for the code-oriented.
<br />
<br />
## The DreamCheekyLEDExtras Library

This adds an ImageLoader class into the mix, using the SkiaSharp nuget package, which adds a helper class for FrameData and two methods that allow images to be loaded directly and displayed. With this, any fully-black, opaque pixel is read as "LED turned off". While intended for images (like GIF or PNG) 21px wide by 7px tall, it also works for different scales. If you create a GIF that's 84 x 28 for example (4x scaling), it will read the top-left-most pixel, then the pixel at position 5, 9, 13, etc. No more tiny thumbnails on high DPI displays!

While this library simplifies the DreamCheekyLED library even further -- and also compiles into a DLL that can be referenced without a direct need to compile C# -- it's still oriented towards higher-level (or just entry-level) coding projects. This, at least, still requires you to *code* to get anything to display.

Enter...
<br />
<br />
## The DreamCheekyPathLoaderApp
This is for either the end user who knows nothing about code, or someone that just finds interfacing with a command prompt script *that* much easier (I haven't been there often, but I *have* been there). It opens a command prompt window that accepts a path to an image as an argument if you want, but opens with a prompt to give it an image to load. Once a suitable image file is supplied, it will display it on the LED Message Board, complete with accurate GIF timing! (Unless you set GIF timing to zero -- please don't do this, I cannot promise *anything* with it as there's still a 1ms delay between screen refreshes to not overload the CPU on lower-end devices). It'll immediately allow you to enter a new path to a new image, and upon receiving a new image, it will display that image instead. Pressing "Q" (uppercase or lowercase) exits.

This is the most "entry-level" method available, allowing anyone to just open an EXE file and display an image. For anything more technical, those options exist too, with plenty of documentation walking you through every step of the way.
<br />
<br />
## The name: Dream Cheeky, or Dream Link?

Short answer: yes, and you might see it used interchangably within this library.

Long answer: It appears they operated under both names with one web domain for each. Long since defunct, Dream Link appears to have been the hardware manufacturer, while Dream Cheeky was the storefront for Dream Link. On the back, it's labeled as:
> DREAM Cheeky<br />
> www.dreamcheeky.com<br />
> DREAMLINK (tm) 2008 (c)<br />
> www.dreamlink.info<br />
> Made in China

The hardware itself of the unit I have identifies through USB as a v1.0.1 -- I do not know if other revisions exist, or if they differ at all.

Interestingly, while the back of many of their boxes claimed they applied for a US Trademark for their names and logos, I was unable to find any such application through the US Patent and Trademark Office TESS system. While they are based in Hong Kong, it appears Sony had an active trademark and logo for the name DreamLink that is still active as of 2026, in the same trademark category as the devices these sold under. I was unable to find any US trademark application for "Dream Cheeky" however, nor for the logo. The Dream Cheeky Facebook page stopped being updated over a decade ago. While no US trademark applies, copyright law may still be in effect for their logos. I have chosen to include them for reference purposes only in the /logos subdirectory of the documentation, without displaying them. There are no logos of any kind included within the code base itself. There is no affiliation between this project and Dreamlink Ltd, Dream Cheeky, Sony Corporation of America, et al.
<br />
<br />
## The non-issue: ChatGPT
This library was built by a real senior-level full stack systems architect with six years of experience at the professional level, vocational school and a college degree ahead of that, and many more years after of various coding projects, most of which are open source under BSD/MIT. None of what I publish is ***ever*** "vibe coded." I use AI specifically to help guide me on the finer points, and learn from the conglomerate of knowledge that it has immediate access to. I treat ChatGPT like a coworker, not a subordinate, and especially not something to plagiarize.


If that type of use of AI bothers you, grow up and quit being a toddler. The same petulance to get rid of the internet as a whole was fruitless too, no matter how many hoards of grumps hated it. Video games, social media, auction sites, online shopping, and more all survived too. It's a consistent recipe: something *new* happens, a bunch of unserious, emotionally immature complainers throw all sorts of words at trying to justify why it's "bad" (completely ignoring the many things they rather enjoy keeping that apply *more* to those "bad" things -- hence "unserious"), because god forbid they accept that maybe their emotions could be *wrong* at all. It's just another form of "othering", throwing negativity towards that which is *different*, *new*, *uncomfortable*, even if it's here to stay. There's a reason people like this exist very largely in politically conservative circles -- othering is one of the many core components of racism, antisemitism, xenophobia, homophobia, etc., after all -- seeing something *new and unusual*, giving you *discomfort*, which must make that thing *inherently bad* somehow.

That flavor of AI use still bothering you? Be honest, you're not angry about the plastic polymer manufacturing industry "using all the water" or you would be throwing a tantrum over 3D printing filaments. You're not angry that Google Search, Bing, or DuckDuckGo exist either or we'd hear about that too. Vague abstract statements like it being "inherently harmful" are just a "nuh-uh" when the adults in the room tell you you're wrong. There is no amount of greater good that would outweigh your *feels*, made worse by whatever miserable life circumstances you have, because emotional immaturity means you need a boogeyman to get mad at instead of just going "man it sucks that RAM, SSD, and GPU prices are super expensive and I can't afford anything anymore" and leaving it at that. Nonono, someone must be to *blame* for how you feel, but no matter what, it *cannot, under any circumstances*, be you somehow. It's *someone else's* fault that you don't feel good. Fun fact: there are genuinely studies on this phenomena, and it leads to isolation, where everyone that gets blamed just up and leaves, until people like you -- those blaming everyone else but themselves -- are left to realize that they've been the problem the whole time. And slowly, but surely, the isolation eats at them until something has to give. Either they grow up and emotionally mature, or it kills them. And hey, the world could use more emotionally mature adults, so stay alive, get on top of the petulance with a therapist and let's never speak of this if ever we meet.

No matter what, in the meantime, I'll do what most of us who have been genuinely contributing to the world prior to AI's arrival are continuing to do -- more of the same, without a care to *who* or *what* benefits from it. My moral compass: ***anything*** I create (and yes, I created ***all*** of this, regardless of what Google replacement I chose to use) that someone or something finds worthwhile enough to use, benefit from, be inspired by, hell even just simply feel *good* about -- is entirely worthwhile. Anyone that *doesn't* like it -- I just don't care really. Grumpy people are everywhere, and have been for my whole life. I'm used to tuning it out already, even if it means I write things like this as a pre-filter so people keep their spouting-off elsewhere :) I share this stuff because I can. I made it, and it shows in plenty of spots in this library.

Can ChatGPT or Claude make this too? Sure, I have no doubt. But -- at least in 2026 -- not in a way that's as intuitive, easy-to-use, and easy-to-understand as I have. At least, not without *major* rewriting to get there. I expect that to get better with time. Me on the other hand, I believe in sharing for all, no matter who. Hobbyist, professional, business, open source, closed source, even AI models, if my code helps someone accomplish something, or learn from it, that's good enough for me. Especially if it leads to seeing others expand on the idea and go further. Or even adapting my approach to new endeavors. I license under BSD/MIT for this exact reason. All I ask is that you leave a link somewhere to this library wherever you may use it. It was hard enough to find anything that even tried to work, let alone actually did. My hope is that no one gets discouraged anymore with this. I've truly built one I'm proud of.