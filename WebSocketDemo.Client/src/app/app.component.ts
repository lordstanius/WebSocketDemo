import { Component, OnInit, OnDestroy } from '@angular/core';
import { DataService } from './core/data.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {

  stockQuote: number;
  sub: Subscription;

  constructor(private dataService: DataService) { }

  ngOnInit() {
    this.sub = this.dataService.getQuotes()
      .subscribe(quote => {
        this.stockQuote = quote;
      });
  }

  sendMessage(message: string) {
    this.dataService.socket.send(message);
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
