import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  public currentCount = 0;

  constructor(private http: HttpClient) { }

  public incrementCounter() {
    this.currentCount++;

    // Generate random HTML (you can replace this with your own logic)
    const randomHtml = `<h1>Random HTML</h1><p>${Math.random()}</p>`;

    // Send the random HTML to the controller method
    this.http.post('http://localhost:5041/qlhw/print', randomHtml, { responseType: 'text' })
      .subscribe(response => {
        console.log('HTML sent successfully:', response);
      }, error => {
        console.error('Error sending HTML:', error);
      });
  }
}
