import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { StaticContentComponent, StaticContentTypes } from "@core";
import { PanelModule } from "primeng/panel";

@Component({
  standalone: true,
  templateUrl: "./page.component.html",
  imports: [CommonModule, PanelModule, StaticContentComponent],
})
export class PageComponent {
    contentType: StaticContentTypes = "Html";
    // content = `
    // <p-card-control class="w-96" header="Card Header" subtitle="Card Subtitle">
    //   <p>Card Content</p>
    //   </p-card-control>

    // <user-id-control userName="admin"></user-id-control>
    //             <user-id-control userName="admin"></user-id-control>
    //            `;
  content = "<strong>Hello, world!</strong> This is some <em>HTML</em> content.";
  //content = "# Markdown Content\n\nThis is some **bold** text and this is *italic* text.\n\n- Item 1\n- Item 2\n- Item 3\n\n[Link to OpenAI](https://www.openai.com)";
}
