import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [],
  styleUrls: ['test.component.scss'],
  templateUrl: 'test.component.html'
})

export class TestComponent {
  constructor() {
    console.log('test');
  }
}
