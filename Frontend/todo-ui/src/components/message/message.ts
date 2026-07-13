import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message.html',
  styleUrl: './message.css'
})
export class MessageComponent {

  readonly successMessage = input('');

  readonly errorMessage = input('');
}