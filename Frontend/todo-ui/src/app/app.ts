import { Component } from '@angular/core';

import { HomeComponent } from './home/home';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    HomeComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

}
