import { Component } from '@angular/core';
import { LayoutComponent } from "./layout/layout.component";

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [LayoutComponent]
})
export class AppComponent {
  title = 'Identity';
}
