from django.views.generic import ListView, DeleteView, CreateView
from models import SampleItem
from forms import AddItemForm


class SampleItemListView(ListView):
    template_name = 'index.html'

    def get_context_data(self, **kwargs):
        context = super(SampleItemListView, self).get_context_data(**kwargs)
        context['add_item_form'] = AddItemForm()
        return context

    def get_queryset(self):
        return SampleItem.objects.all()


class ItemDeleteView(DeleteView):
    model = SampleItem
    success_url = "/"


class ItemCreateView(CreateView):
    form_class = AddItemForm
    model = SampleItem
    success_url = "/"