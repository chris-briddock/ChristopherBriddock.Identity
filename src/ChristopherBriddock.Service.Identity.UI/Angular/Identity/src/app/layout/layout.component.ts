import { Component } from '@angular/core';
import { HomeComponent } from '../home/home.component';
import { NavComponent } from "../nav/nav.component";
import { FooterComponent } from "../footer/footer.component";
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-layout',
    standalone: true,
    templateUrl: './layout.component.html',
    styleUrl: './layout.component.css',
    imports: [HomeComponent, NavComponent, FooterComponent, RouterOutlet]
})
export class LayoutComponent {

}
